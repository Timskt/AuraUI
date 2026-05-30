#!/usr/bin/env python3
"""
Apply CodexSwitchUI interaction patterns to ALL AuraUI AXAML theme files.

Patterns:
1. Hover: Opacity 0.92 on control; remove Background from :pointerover styles
2. Pressed: Opacity 0.86 + scale(0.98) on control; remove Background from :pressed styles
3. Focus: :focus-visible instead of :focus
4. Disabled: Opacity 0.4 only; remove Background/Foreground
"""

import re
import os
import glob

BASE = '/Users/sky/code/vibe/ui/src'
DIRECTORIES = [
    os.path.join(BASE, 'AuraUI.Themes.Fluent/Controls'),
    os.path.join(BASE, 'AuraUI.Themes.Material/Controls'),
]

PSEUDO_RE = re.compile(
    r':(pointerover|pressed|focus-visible|focus|disabled|checked|selected|'
    r'expanded|indeterminate|readonly|error|not-empty|empty|horizontal|vertical|focused)'
)


def find_axaml_files():
    files = []
    for d in DIRECTORIES:
        files.extend(glob.glob(os.path.join(d, '*.axaml')))
    return sorted(files)


def extract_selector(line):
    m = re.search(r'Selector="([^"]*)"', line)
    return m.group(1) if m else None


def get_control_part(sel):
    if '/template/' in sel:
        return sel.split('/template/')[0].strip()
    return sel.strip()


def get_template_part(sel):
    if '/template/' in sel:
        return sel.split('/template/', 1)[1].strip()
    return ''


def pseudo_is_on_template_target(sel):
    if '/template/' not in sel:
        return False
    template_part = get_template_part(sel)
    return ':pointerover' in template_part or ':pressed' in template_part


def get_clean_control_name(sel):
    ctrl = get_control_part(sel)
    return PSEUDO_RE.sub('', ctrl).strip()


def read_style_block(lines, start):
    block = [lines[start]]
    depth = 1
    i = start + 1
    while i < len(lines) and depth > 0:
        line = lines[i]
        block.append(line)
        opens = len(re.findall(r'<Style\b', line))
        closes = len(re.findall(r'</Style>', line))
        depth += opens - closes
        i += 1
    return block, i


def block_has_setter(block, prop):
    pattern = re.compile(r'<Setter\s+Property="' + re.escape(prop) + r'"')
    return any(pattern.search(line) for line in block)


def block_meaningful_setter_count(block):
    """Count setters that are NOT just CornerRadius."""
    count = 0
    for line in block:
        m = re.search(r'<Setter\s+Property="(\w+)"', line)
        if m and m.group(1) != 'CornerRadius':
            count += 1
    return count


def remove_setter(block, prop):
    pattern = re.compile(r'<Setter\s+Property="' + re.escape(prop) + r'"')
    new_block = []
    removed = False
    for line in block:
        if pattern.search(line):
            removed = True
        else:
            new_block.append(line)
    return new_block, removed


def get_indent(block):
    for line in block:
        m = re.match(r'^(\s*)', line)
        if m and m.group(1):
            return m.group(1)
    return '    '


def insert_before_close(block, setter_line):
    result = []
    for line in block:
        if '</Style>' in line:
            result.append(setter_line)
        result.append(line)
    return result


def ensure_opacity(block, value):
    """Add Opacity setter if not already present."""
    if block_has_setter(block, 'Opacity'):
        return block
    indent = get_indent(block)
    return insert_before_close(block, f'{indent}    <Setter Property="Opacity" Value="{value}"/>')


def ensure_scale_098(block):
    """Ensure ScaleTransform.ScaleX/Y are 0.98."""
    has_x = block_has_setter(block, 'ScaleTransform.ScaleX')
    has_y = block_has_setter(block, 'ScaleTransform.ScaleY')

    # Update existing values
    new_block = []
    for line in block:
        if 'ScaleTransform.ScaleX' in line or 'ScaleTransform.ScaleY' in line:
            line = re.sub(r'Value="0\.9[5-7]"', 'Value="0.98"', line)
        new_block.append(line)

    # Add missing
    indent = get_indent(new_block)
    result = []
    for line in new_block:
        if '</Style>' in line:
            if not has_x:
                result.append(f'{indent}    <Setter Property="ScaleTransform.ScaleX" Value="0.98"/>')
            if not has_y:
                result.append(f'{indent}    <Setter Property="ScaleTransform.ScaleY" Value="0.98"/>')
        result.append(line)
    return result


def make_pointerover_opacity_block(ctrl_sel, indent='    '):
    sel = f'{ctrl_sel}:pointerover' if ':pointerover' not in ctrl_sel else ctrl_sel
    return [
        f'{indent}<Style Selector="{sel}">',
        f'{indent}    <Setter Property="Opacity" Value="0.92"/>',
        f'{indent}</Style>',
    ]


def make_pressed_opacity_block(ctrl_sel, indent='    '):
    sel = f'{ctrl_sel}:pressed' if ':pressed' not in ctrl_sel else ctrl_sel
    return [
        f'{indent}<Style Selector="{sel}">',
        f'{indent}    <Setter Property="Opacity" Value="0.86"/>',
        f'{indent}    <Setter Property="ScaleTransform.ScaleX" Value="0.98"/>',
        f'{indent}    <Setter Property="ScaleTransform.ScaleY" Value="0.98"/>',
        f'{indent}</Style>',
    ]


def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content
    lines = content.split('\n')
    result = []
    i = 0
    changes = []

    controls_needing_pointerover = set()
    controls_needing_pressed = set()

    while i < len(lines):
        line = lines[i]
        sel = extract_selector(line)

        if sel and '</Style>' not in line and '<Style ' in line:
            block, end_idx = read_style_block(lines, i)
            ctrl_part = get_control_part(sel)
            clean_ctrl = get_clean_control_name(sel)
            pseudo_on_tmpl = pseudo_is_on_template_target(sel)
            is_tmpl = '/template/' in sel
            is_po = ':pointerover' in sel
            is_pr = ':pressed' in sel

            # === FIX 1 & 2: Remove Background from :pointerover/:pressed styles ===
            if (is_po or is_pr) and block_has_setter(block, 'Background'):
                new_block, removed = remove_setter(block, 'Background')
                if removed:
                    pseudo_type = 'pointerover' if is_po else 'pressed'
                    changes.append(f'Removed Background from {pseudo_type}: {sel}')

                    if not is_tmpl:
                        # CONTROL-LEVEL: Add Opacity + scale, always keep block
                        if is_po:
                            new_block = ensure_opacity(new_block, '0.92')
                        else:
                            new_block = ensure_opacity(new_block, '0.86')
                            new_block = ensure_scale_098(new_block)
                        result.extend(new_block)
                    elif pseudo_on_tmpl:
                        # TEMPLATE with pseudo on template target: add Opacity to same block
                        if is_po:
                            new_block = ensure_opacity(new_block, '0.92')
                        else:
                            new_block = ensure_opacity(new_block, '0.86')
                        if block_meaningful_setter_count(new_block) > 0:
                            result.extend(new_block)
                    else:
                        # TEMPLATE with pseudo on control: keep if meaningful, track for new style
                        if block_meaningful_setter_count(new_block) > 0:
                            result.extend(new_block)
                        if is_po:
                            controls_needing_pointerover.add(clean_ctrl)
                        else:
                            controls_needing_pressed.add(clean_ctrl)

                    i = end_idx
                    continue

            # === FIX 3: Convert :focus to :focus-visible ===
            if ':focus' in sel and ':focus-visible' not in sel and ':focus-within' not in sel:
                new_sel = sel.replace(':focus', ':focus-visible')
                new_block = []
                for bl in block:
                    bl = bl.replace(f'Selector="{sel}"', f'Selector="{new_sel}"')
                    new_block.append(bl)
                changes.append(f'Converted :focus to :focus-visible: {sel}')
                result.extend(new_block)
                i = end_idx
                continue

            # === FIX 4: Simplify :disabled styles ===
            if ':disabled' in sel and not is_tmpl:
                had_bg = block_has_setter(block, 'Background')
                had_fg = block_has_setter(block, 'Foreground')
                if had_bg or had_fg:
                    new_block = block
                    new_block, _ = remove_setter(new_block, 'Background')
                    new_block, _ = remove_setter(new_block, 'Foreground')
                    if not block_has_setter(new_block, 'Opacity'):
                        new_block = ensure_opacity(new_block, '0.4')
                    changes.append(f'Simplified disabled: {sel}')
                    result.extend(new_block)
                    i = end_idx
                    continue

            # === FIX 5: Add Opacity to existing :pressed on control (not template) ===
            if is_pr and not is_tmpl:
                new_block = block
                new_block = ensure_opacity(new_block, '0.86')
                new_block = ensure_scale_098(new_block)
                changes.append(f'Added Opacity to pressed control: {sel}')
                result.extend(new_block)
                i = end_idx
                continue

            # Default: keep block as-is
            result.extend(block)
            i = end_idx
            continue

        result.append(line)
        i += 1

    # === FIX 6: Add opacity styles for controls that need them ===
    # Pre-scan result for existing control-level styles
    result_pointerover = set()
    result_pressed = set()
    for line in result:
        s = extract_selector(line)
        if s and '/template/' not in s:
            clean = get_clean_control_name(s)
            if ':pointerover' in s:
                result_pointerover.add(clean)
            if ':pressed' in s:
                result_pressed.add(clean)

    new_style_lines = []
    for ctrl in sorted(controls_needing_pointerover):
        if ctrl not in result_pointerover:
            new_style_lines.extend(make_pointerover_opacity_block(ctrl))
            new_style_lines.append('')
            changes.append(f'Added pointerover opacity for: {ctrl}')

    for ctrl in sorted(controls_needing_pressed):
        if ctrl not in result_pressed:
            new_style_lines.extend(make_pressed_opacity_block(ctrl))
            new_style_lines.append('')
            changes.append(f'Added pressed opacity for: {ctrl}')

    if new_style_lines:
        final_result = []
        inserted = False
        for line in result:
            if '</Styles>' in line and not inserted:
                final_result.append('')
                final_result.append('    <!-- ============================================================ -->')
                final_result.append('    <!-- CodexSwitchUI: Opacity-based interaction states              -->')
                final_result.append('    <!-- ============================================================ -->')
                final_result.extend(new_style_lines)
                inserted = True
            final_result.append(line)
        result = final_result

    content_str = '\n'.join(result)

    if content_str != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content_str)
        return changes
    return []


def main():
    files = find_axaml_files()
    print(f'Processing {len(files)} AXAML files...\n')

    total_changes = 0
    modified_files = 0

    for filepath in files:
        changes = process_file(filepath)
        if changes:
            modified_files += 1
            total_changes += len(changes)
            rel = os.path.relpath(filepath, BASE)
            print(f'  {rel}:')
            for c in changes:
                print(f'    - {c}')

    print(f'\nDone: {modified_files} files modified, {total_changes} total changes')


if __name__ == '__main__':
    main()

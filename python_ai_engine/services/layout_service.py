"""
Layout services: suggest, optimize, validate.
Provides intelligent object placement and layout computation for SCADA graphic modules.

Strategies:
1. Grid layout: evenly spaced rows/columns
2. Dashboard layout: header/body/footer zones
3. Flow layout: sequential horizontal/vertical placement
4. Optimization: overlap removal, alignment snapping, spacing normalization
"""

import logging
import math
import time
from typing import Dict, List, Optional, Tuple

logger = logging.getLogger(__name__)

# ---------------------------------------------------------------------------
# Layout constants
# ---------------------------------------------------------------------------

# Default margins and spacing
DEFAULT_MARGIN = 20
DEFAULT_SPACING = 15
DEFAULT_HEADER_HEIGHT = 60
DEFAULT_LABEL_HEIGHT = 25

# Tag type → default object dimensions
TAG_TYPE_DIMENSIONS: Dict[str, Dict[str, int]] = {
    'AI': {'width': 180, 'height': 70, 'label_height': DEFAULT_LABEL_HEIGHT},
    'AO': {'width': 200, 'height': 90, 'label_height': DEFAULT_LABEL_HEIGHT},
    'DI': {'width': 120, 'height': 60, 'label_height': DEFAULT_LABEL_HEIGHT},
    'DO': {'width': 150, 'height': 80, 'label_height': DEFAULT_LABEL_HEIGHT},
    'ST': {'width': 200, 'height': 50, 'label_height': DEFAULT_LABEL_HEIGHT},
    'TREND': {'width': 400, 'height': 250, 'label_height': 30},
    'ALARM': {'width': 400, 'height': 200, 'label_height': 30},
    'GAUGE': {'width': 150, 'height': 150, 'label_height': DEFAULT_LABEL_HEIGHT},
    'DEFAULT': {'width': 150, 'height': 70, 'label_height': DEFAULT_LABEL_HEIGHT},
}

# Dashboard zone ratios (header, body, footer)
DASHBOARD_ZONES = {
    'header': {'ratio': 0.08, 'min_height': 50},
    'body': {'ratio': 0.72, 'min_height': 200},
    'footer': {'ratio': 0.20, 'min_height': 100},
}


# ---------------------------------------------------------------------------
# Helper functions
# ---------------------------------------------------------------------------

def _get_dimensions(obj_type: str, tag_type: str = None) -> Dict[str, int]:
    """Get default dimensions for an object type."""
    if obj_type and obj_type.upper() in TAG_TYPE_DIMENSIONS:
        return dict(TAG_TYPE_DIMENSIONS[obj_type.upper()])
    if tag_type and tag_type.upper() in TAG_TYPE_DIMENSIONS:
        return dict(TAG_TYPE_DIMENSIONS[tag_type.upper()])
    return dict(TAG_TYPE_DIMENSIONS['DEFAULT'])


def _compute_grid(elements: List[dict], container: dict,
                  columns: int = 0, spacing: int = DEFAULT_SPACING,
                  margin: int = DEFAULT_MARGIN) -> List[dict]:
    """Compute grid layout positions."""
    if not elements:
        return []

    c_width = container.get('width', 1920)
    c_height = container.get('height', 1080)

    # Auto-compute columns if not specified
    if columns <= 0:
        # Aim for roughly square cells
        n = len(elements)
        columns = max(1, min(n, int(math.ceil(math.sqrt(n)))))

    rows = math.ceil(len(elements) / columns)

    # Compute cell dimensions
    avail_width = c_width - 2 * margin - (columns - 1) * spacing
    avail_height = c_height - 2 * margin - (rows - 1) * spacing
    cell_width = avail_width // columns
    cell_height = avail_height // rows

    positions = []
    for i, elem in enumerate(elements):
        row = i // columns
        col = i % columns

        # Use element's own dimensions or fit to cell
        dims = _get_dimensions(
            elem.get('object_type', ''),
            elem.get('tag_type', '')
        )
        obj_w = min(elem.get('width', dims['width']), cell_width)
        obj_h = min(elem.get('height', dims['height']), cell_height)

        # Center in cell
        cell_x = margin + col * (cell_width + spacing)
        cell_y = margin + row * (cell_height + spacing)
        x = cell_x + (cell_width - obj_w) // 2
        y = cell_y + (cell_height - obj_h) // 2

        positions.append({
            'id': elem.get('id', i),
            'x': x,
            'y': y,
            'width': obj_w,
            'height': obj_h,
            'row': row,
            'col': col,
        })

    return positions


def _compute_flow(elements: List[dict], container: dict,
                  direction: str = 'horizontal',
                  spacing: int = DEFAULT_SPACING,
                  margin: int = DEFAULT_MARGIN,
                  wrap: bool = True) -> List[dict]:
    """Compute flow layout (horizontal or vertical with optional wrapping)."""
    if not elements:
        return []

    c_width = container.get('width', 1920)
    c_height = container.get('height', 1080)

    positions = []
    x = margin
    y = margin
    row_max_height = 0
    col_max_width = 0

    for i, elem in enumerate(elements):
        dims = _get_dimensions(
            elem.get('object_type', ''),
            elem.get('tag_type', '')
        )
        obj_w = elem.get('width', dims['width'])
        obj_h = elem.get('height', dims['height'])

        if direction == 'horizontal':
            # Wrap to next row if exceeds container width
            if wrap and x + obj_w > c_width - margin and i > 0:
                x = margin
                y += row_max_height + spacing
                row_max_height = 0

            positions.append({
                'id': elem.get('id', i),
                'x': x,
                'y': y,
                'width': obj_w,
                'height': obj_h,
            })

            x += obj_w + spacing
            row_max_height = max(row_max_height, obj_h)

        else:  # vertical
            if wrap and y + obj_h > c_height - margin and i > 0:
                y = margin
                x += col_max_width + spacing
                col_max_width = 0

            positions.append({
                'id': elem.get('id', i),
                'x': x,
                'y': y,
                'width': obj_w,
                'height': obj_h,
            })

            y += obj_h + spacing
            col_max_width = max(col_max_width, obj_w)

    return positions


def _compute_dashboard(elements: List[dict], container: dict,
                       spacing: int = DEFAULT_SPACING,
                       margin: int = DEFAULT_MARGIN) -> List[dict]:
    """
    Compute dashboard-style layout with zones:
    - Header: title/label area (top)
    - Body: main display objects (middle, grid arrangement)
    - Footer: trend/alarm/summary (bottom)
    """
    if not elements:
        return []

    c_width = container.get('width', 1920)
    c_height = container.get('height', 1080)

    # Classify elements into zones
    header_items = []
    body_items = []
    footer_items = []

    for elem in elements:
        obj_type = (elem.get('object_type', '') or '').upper()
        tag_type = (elem.get('tag_type', '') or '').upper()
        zone = elem.get('zone', '')

        if zone == 'header':
            header_items.append(elem)
        elif zone == 'footer':
            footer_items.append(elem)
        elif zone == 'body':
            body_items.append(elem)
        elif obj_type in ('TREND', 'MULTI_TREND', 'CHART', 'ALARM', 'ALARM_LIST'):
            footer_items.append(elem)
        elif obj_type in ('TITLE', 'LABEL', 'HEADER'):
            header_items.append(elem)
        else:
            body_items.append(elem)

    # Compute zone heights
    header_h = max(
        DASHBOARD_ZONES['header']['min_height'],
        int(c_height * DASHBOARD_ZONES['header']['ratio'])
    ) if header_items else 0

    footer_h = max(
        DASHBOARD_ZONES['footer']['min_height'],
        int(c_height * DASHBOARD_ZONES['footer']['ratio'])
    ) if footer_items else 0

    body_h = c_height - header_h - footer_h

    positions = []

    # Layout header items (horizontal flow)
    if header_items:
        header_container = {'width': c_width, 'height': header_h}
        header_pos = _compute_flow(header_items, header_container,
                                   direction='horizontal', spacing=spacing, margin=margin)
        for p in header_pos:
            # Center vertically in header zone
            p['y'] = margin + (header_h - p['height']) // 2
            p['zone'] = 'header'
            positions.append(p)

    # Layout body items (grid)
    if body_items:
        body_container = {'width': c_width, 'height': body_h}
        body_pos = _compute_grid(body_items, body_container,
                                 spacing=spacing, margin=margin)
        for p in body_pos:
            p['y'] += header_h
            p['zone'] = 'body'
            positions.append(p)

    # Layout footer items (horizontal flow, stretch width)
    if footer_items:
        footer_container = {'width': c_width, 'height': footer_h}
        # Give footer items more width
        for item in footer_items:
            dims = _get_dimensions(item.get('object_type', ''), item.get('tag_type', ''))
            if 'width' not in item:
                item['width'] = max(dims['width'], (c_width - 2 * margin - (len(footer_items) - 1) * spacing) // max(1, len(footer_items)))
            if 'height' not in item:
                item['height'] = max(dims['height'], footer_h - 2 * margin)

        footer_pos = _compute_flow(footer_items, footer_container,
                                   direction='horizontal', spacing=spacing, margin=margin)
        for p in footer_pos:
            p['y'] += header_h + body_h
            p['zone'] = 'footer'
            positions.append(p)

    return positions


def _snap_to_grid(positions: List[dict], grid_size: int = 10) -> List[dict]:
    """Snap positions to nearest grid point."""
    for p in positions:
        p['x'] = round(p['x'] / grid_size) * grid_size
        p['y'] = round(p['y'] / grid_size) * grid_size
    return positions


def _detect_overlaps(positions: List[dict]) -> List[dict]:
    """Detect overlapping objects."""
    overlaps = []
    for i in range(len(positions)):
        for j in range(i + 1, len(positions)):
            a = positions[i]
            b = positions[j]
            if (a['x'] < b['x'] + b['width'] and
                a['x'] + a['width'] > b['x'] and
                a['y'] < b['y'] + b['height'] and
                a['y'] + a['height'] > b['y']):
                overlaps.append({
                    'element_a': a.get('id'),
                    'element_b': b.get('id'),
                    'overlap_x': min(a['x'] + a['width'], b['x'] + b['width']) - max(a['x'], b['x']),
                    'overlap_y': min(a['y'] + a['height'], b['y'] + b['height']) - max(a['y'], b['y']),
                })
    return overlaps


def _resolve_overlaps(positions: List[dict], container: dict,
                      spacing: int = DEFAULT_SPACING) -> List[dict]:
    """Push overlapping objects apart."""
    max_iterations = 50
    c_width = container.get('width', 1920)
    c_height = container.get('height', 1080)

    for _ in range(max_iterations):
        overlaps = _detect_overlaps(positions)
        if not overlaps:
            break

        for overlap in overlaps:
            a = next((p for p in positions if p.get('id') == overlap['element_a']), None)
            b = next((p for p in positions if p.get('id') == overlap['element_b']), None)
            if not a or not b:
                continue

            # Push the second element right or down
            if overlap['overlap_x'] < overlap['overlap_y']:
                # Less horizontal overlap — push horizontally
                b['x'] = a['x'] + a['width'] + spacing
            else:
                # Less vertical overlap — push vertically
                b['y'] = a['y'] + a['height'] + spacing

            # Clamp to container
            b['x'] = max(0, min(b['x'], c_width - b['width']))
            b['y'] = max(0, min(b['y'], c_height - b['height']))

    return positions


def _compute_alignment_guides(positions: List[dict]) -> List[dict]:
    """Compute alignment guide lines between nearby objects."""
    guides = []
    threshold = 5  # pixels

    for i in range(len(positions)):
        for j in range(i + 1, len(positions)):
            a = positions[i]
            b = positions[j]

            # Left edge alignment
            if abs(a['x'] - b['x']) <= threshold:
                guides.append({
                    'type': 'vertical',
                    'x': a['x'],
                    'elements': [a.get('id'), b.get('id')],
                    'alignment': 'left',
                })

            # Right edge alignment
            a_right = a['x'] + a['width']
            b_right = b['x'] + b['width']
            if abs(a_right - b_right) <= threshold:
                guides.append({
                    'type': 'vertical',
                    'x': a_right,
                    'elements': [a.get('id'), b.get('id')],
                    'alignment': 'right',
                })

            # Top edge alignment
            if abs(a['y'] - b['y']) <= threshold:
                guides.append({
                    'type': 'horizontal',
                    'y': a['y'],
                    'elements': [a.get('id'), b.get('id')],
                    'alignment': 'top',
                })

            # Center X alignment
            a_cx = a['x'] + a['width'] // 2
            b_cx = b['x'] + b['width'] // 2
            if abs(a_cx - b_cx) <= threshold:
                guides.append({
                    'type': 'vertical',
                    'x': a_cx,
                    'elements': [a.get('id'), b.get('id')],
                    'alignment': 'center_x',
                })

            # Center Y alignment
            a_cy = a['y'] + a['height'] // 2
            b_cy = b['y'] + b['height'] // 2
            if abs(a_cy - b_cy) <= threshold:
                guides.append({
                    'type': 'horizontal',
                    'y': a_cy,
                    'elements': [a.get('id'), b.get('id')],
                    'alignment': 'center_y',
                })

    return guides


# ---------------------------------------------------------------------------
# Service handlers
# ---------------------------------------------------------------------------

async def handle_layout_suggest(payload: dict) -> dict:
    """
    layout/suggest — Suggest optimal layout for SCADA objects.

    Input: {
        canvas_size: {width, height},       # Module canvas dimensions
        objects: [{                          # Objects to place
            id: str|int,
            object_type: str,               # TEXT, GAUGE, TREND, ALARM, etc.
            tag_type: str,                  # AI, AO, DI, DO, ST (optional)
            tag: str,                       # Tag name (optional)
            label: str,                     # Display label (optional)
            width: int,                     # Override width (optional)
            height: int,                    # Override height (optional)
            zone: str,                      # header|body|footer (optional)
        }],
        existing_objects: [{x, y, width, height}],  # Already placed (optional)
        algorithm: str,                     # grid|flow|dashboard|auto (default: auto)
        options: {
            columns: int,                   # Grid columns (optional)
            direction: str,                 # horizontal|vertical (flow only)
            spacing: int,                   # Object spacing (default: 15)
            margin: int,                    # Canvas margin (default: 20)
            snap_to_grid: bool,             # Snap to grid (default: true)
            grid_size: int,                 # Grid size (default: 10)
        }
    }

    Output: {
        positions: [{id, x, y, width, height, ...}],
        algorithm_used: str,
        guides: [{type, x|y, elements, alignment}],
        score: float,                       # Layout quality score 0~1
    }
    """
    payload = payload or {}
    start_time = time.monotonic()

    canvas = payload.get('canvas_size', {'width': 1920, 'height': 1080})
    objects = payload.get('objects', [])
    existing = payload.get('existing_objects', [])
    algorithm = payload.get('algorithm', 'auto')
    options = payload.get('options', {})

    if not objects:
        return {
            'positions': [],
            'algorithm_used': 'none',
            'guides': [],
            'score': 1.0,
            'computation_ms': 0,
        }

    spacing = options.get('spacing', DEFAULT_SPACING)
    margin = options.get('margin', DEFAULT_MARGIN)
    columns = options.get('columns', 0)
    direction = options.get('direction', 'horizontal')
    snap = options.get('snap_to_grid', True)
    grid_size = options.get('grid_size', 10)

    # Auto-select algorithm
    if algorithm == 'auto':
        has_trend_or_alarm = any(
            (o.get('object_type', '') or '').upper() in ('TREND', 'MULTI_TREND', 'CHART', 'ALARM', 'ALARM_LIST')
            for o in objects
        )
        if has_trend_or_alarm and len(objects) >= 3:
            algorithm = 'dashboard'
        elif len(objects) <= 6:
            algorithm = 'flow'
        else:
            algorithm = 'grid'

    # Compute layout
    if algorithm == 'grid':
        positions = _compute_grid(objects, canvas, columns=columns,
                                  spacing=spacing, margin=margin)
    elif algorithm == 'flow':
        positions = _compute_flow(objects, canvas, direction=direction,
                                  spacing=spacing, margin=margin)
    elif algorithm == 'dashboard':
        positions = _compute_dashboard(objects, canvas,
                                       spacing=spacing, margin=margin)
    else:
        positions = _compute_grid(objects, canvas, spacing=spacing, margin=margin)

    # Avoid existing objects
    if existing:
        all_positions = list(existing) + positions
        positions = _resolve_overlaps(positions, canvas, spacing)

    # Snap to grid
    if snap:
        positions = _snap_to_grid(positions, grid_size)

    # Compute alignment guides
    guides = _compute_alignment_guides(positions)

    # Score: penalize overlaps and out-of-bounds
    overlaps = _detect_overlaps(positions)
    oob_count = sum(
        1 for p in positions
        if p['x'] < 0 or p['y'] < 0 or
           p['x'] + p['width'] > canvas.get('width', 1920) or
           p['y'] + p['height'] > canvas.get('height', 1080)
    )
    score = max(0.0, 1.0 - len(overlaps) * 0.15 - oob_count * 0.1)

    duration_ms = (time.monotonic() - start_time) * 1000

    return {
        'positions': positions,
        'algorithm_used': algorithm,
        'guides': guides,
        'score': round(score, 2),
        'computation_ms': round(duration_ms, 1),
    }


async def handle_layout_optimize(payload: dict) -> dict:
    """
    layout/optimize — Optimize existing layout (resolve overlaps, align, normalize spacing).

    Input: {
        canvas_size: {width, height},
        positions: [{id, x, y, width, height}],   # Current positions
        options: {
            resolve_overlaps: bool,                 # Push apart overlapping objects
            align_edges: bool,                      # Snap nearby edges together
            normalize_spacing: bool,                # Even out spacing between objects
            snap_to_grid: bool,
            grid_size: int,
        }
    }

    Output: {
        positions: [{id, x, y, width, height}],   # Optimized positions
        changes: [{id, old_x, old_y, new_x, new_y}],
        overlaps_resolved: int,
        score_before: float,
        score_after: float,
    }
    """
    payload = payload or {}
    start_time = time.monotonic()

    canvas = payload.get('canvas_size', {'width': 1920, 'height': 1080})
    positions = payload.get('positions', [])
    options = payload.get('options', {})

    if not positions:
        return {
            'positions': [],
            'changes': [],
            'overlaps_resolved': 0,
            'score_before': 1.0,
            'score_after': 1.0,
        }

    # Deep copy for comparison
    original = [dict(p) for p in positions]
    optimized = [dict(p) for p in positions]

    # Score before
    overlaps_before = _detect_overlaps(optimized)
    score_before = max(0.0, 1.0 - len(overlaps_before) * 0.15)

    # Resolve overlaps
    overlaps_resolved = 0
    if options.get('resolve_overlaps', True):
        before_count = len(_detect_overlaps(optimized))
        optimized = _resolve_overlaps(optimized, canvas)
        after_count = len(_detect_overlaps(optimized))
        overlaps_resolved = before_count - after_count

    # Align nearby edges
    if options.get('align_edges', True):
        threshold = 8
        for i in range(len(optimized)):
            for j in range(i + 1, len(optimized)):
                a = optimized[i]
                b = optimized[j]
                # Snap left edges
                if abs(a['x'] - b['x']) <= threshold:
                    b['x'] = a['x']
                # Snap top edges
                if abs(a['y'] - b['y']) <= threshold:
                    b['y'] = a['y']

    # Normalize spacing between adjacent objects
    if options.get('normalize_spacing', False):
        # Sort by position and normalize gaps
        sorted_by_x = sorted(optimized, key=lambda p: p['x'])
        for i in range(1, len(sorted_by_x)):
            prev = sorted_by_x[i - 1]
            curr = sorted_by_x[i]
            gap = curr['x'] - (prev['x'] + prev['width'])
            if 0 < gap < DEFAULT_SPACING * 3:
                curr['x'] = prev['x'] + prev['width'] + DEFAULT_SPACING

    # Snap to grid
    if options.get('snap_to_grid', True):
        grid_size = options.get('grid_size', 10)
        optimized = _snap_to_grid(optimized, grid_size)

    # Compute changes
    changes = []
    for orig, opt in zip(original, optimized):
        if orig['x'] != opt['x'] or orig['y'] != opt['y']:
            changes.append({
                'id': opt.get('id'),
                'old_x': orig['x'],
                'old_y': orig['y'],
                'new_x': opt['x'],
                'new_y': opt['y'],
            })

    # Score after
    overlaps_after = _detect_overlaps(optimized)
    score_after = max(0.0, 1.0 - len(overlaps_after) * 0.15)

    duration_ms = (time.monotonic() - start_time) * 1000

    return {
        'positions': optimized,
        'changes': changes,
        'overlaps_resolved': overlaps_resolved,
        'score_before': round(score_before, 2),
        'score_after': round(score_after, 2),
        'computation_ms': round(duration_ms, 1),
    }


async def handle_layout_validate(payload: dict) -> dict:
    """
    layout/validate — Validate layout for issues (overlaps, out-of-bounds, accessibility).

    Input: {
        canvas_size: {width, height},
        positions: [{id, x, y, width, height}],
    }

    Output: {
        valid: bool,
        issues: [{type, severity, element_ids, description}],
        overlap_count: int,
        out_of_bounds_count: int,
    }
    """
    payload = payload or {}
    canvas = payload.get('canvas_size', {'width': 1920, 'height': 1080})
    positions = payload.get('positions', [])

    c_width = canvas.get('width', 1920)
    c_height = canvas.get('height', 1080)
    issues = []

    # Check overlaps
    overlaps = _detect_overlaps(positions)
    for ov in overlaps:
        issues.append({
            'type': 'overlap',
            'severity': 'warning',
            'element_ids': [ov['element_a'], ov['element_b']],
            'description': f"오브젝트 겹침: {ov['overlap_x']}x{ov['overlap_y']}px",
        })

    # Check out-of-bounds
    oob_count = 0
    for p in positions:
        reasons = []
        if p.get('x', 0) < 0:
            reasons.append('좌측 초과')
        if p.get('y', 0) < 0:
            reasons.append('상단 초과')
        if p.get('x', 0) + p.get('width', 0) > c_width:
            reasons.append('우측 초과')
        if p.get('y', 0) + p.get('height', 0) > c_height:
            reasons.append('하단 초과')
        if reasons:
            oob_count += 1
            issues.append({
                'type': 'out_of_bounds',
                'severity': 'error',
                'element_ids': [p.get('id')],
                'description': f"캔버스 영역 벗어남: {', '.join(reasons)}",
            })

    # Check minimum spacing (too close but not overlapping)
    min_spacing = 5
    for i in range(len(positions)):
        for j in range(i + 1, len(positions)):
            a = positions[i]
            b = positions[j]
            # Horizontal gap
            h_gap = max(b['x'] - (a['x'] + a['width']), a['x'] - (b['x'] + b['width']))
            v_gap = max(b['y'] - (a['y'] + a['height']), a['y'] - (b['y'] + b['height']))
            if 0 < h_gap < min_spacing and v_gap < 0:
                issues.append({
                    'type': 'too_close',
                    'severity': 'info',
                    'element_ids': [a.get('id'), b.get('id')],
                    'description': f'오브젝트 간격이 너무 좁습니다 ({h_gap}px)',
                })

    error_count = sum(1 for i in issues if i['severity'] == 'error')

    return {
        'valid': error_count == 0 and len(overlaps) == 0,
        'issues': issues,
        'overlap_count': len(overlaps),
        'out_of_bounds_count': oob_count,
    }


async def handle_layout_smart_place(payload: dict) -> dict:
    """
    layout/smart-place — Find the best position for a single new object,
    considering existing objects.

    Input: {
        canvas_size: {width, height},
        existing_objects: [{x, y, width, height}],  # Already placed objects
        new_object: {
            object_type: str,
            tag_type: str,
            width: int,                             # Optional
            height: int,                            # Optional
        },
        preferred_position: {x, y},                 # Hint (optional, e.g. drop position)
    }

    Output: {
        position: {x, y, width, height},
        reasoning: str,
    }
    """
    payload = payload or {}
    canvas = payload.get('canvas_size', {'width': 1920, 'height': 1080})
    existing = payload.get('existing_objects', [])
    new_obj = payload.get('new_object', {})
    preferred = payload.get('preferred_position', None)

    c_width = canvas.get('width', 1920)
    c_height = canvas.get('height', 1080)

    dims = _get_dimensions(
        new_obj.get('object_type', ''),
        new_obj.get('tag_type', '')
    )
    obj_w = new_obj.get('width', dims['width'])
    obj_h = new_obj.get('height', dims['height'])

    # If preferred position is given and doesn't overlap, use it
    if preferred:
        px = preferred.get('x', DEFAULT_MARGIN)
        py = preferred.get('y', DEFAULT_MARGIN)
        # Clamp to canvas
        px = max(0, min(px, c_width - obj_w))
        py = max(0, min(py, c_height - obj_h))

        overlaps = False
        for ex in existing:
            if (px < ex['x'] + ex['width'] and px + obj_w > ex['x'] and
                py < ex['y'] + ex['height'] and py + obj_h > ex['y']):
                overlaps = True
                break

        if not overlaps:
            return {
                'position': {'x': px, 'y': py, 'width': obj_w, 'height': obj_h},
                'reasoning': '사용자 지정 위치 사용 (겹침 없음)',
            }

    # Strategy: Find nearest empty position
    # Scan grid positions and find the one closest to preferred or center
    grid_step = 20
    best_pos = None
    best_dist = float('inf')
    target_x = preferred['x'] if preferred else c_width // 2
    target_y = preferred['y'] if preferred else c_height // 2

    for scan_y in range(DEFAULT_MARGIN, c_height - obj_h, grid_step):
        for scan_x in range(DEFAULT_MARGIN, c_width - obj_w, grid_step):
            overlaps = False
            for ex in existing:
                if (scan_x < ex['x'] + ex['width'] + DEFAULT_SPACING and
                    scan_x + obj_w + DEFAULT_SPACING > ex['x'] and
                    scan_y < ex['y'] + ex['height'] + DEFAULT_SPACING and
                    scan_y + obj_h + DEFAULT_SPACING > ex['y']):
                    overlaps = True
                    break

            if not overlaps:
                dist = math.sqrt((scan_x - target_x) ** 2 + (scan_y - target_y) ** 2)
                if dist < best_dist:
                    best_dist = dist
                    best_pos = (scan_x, scan_y)

    if best_pos:
        return {
            'position': {'x': best_pos[0], 'y': best_pos[1], 'width': obj_w, 'height': obj_h},
            'reasoning': '기존 오브젝트와 겹치지 않는 가장 가까운 위치',
        }

    # Fallback: bottom of existing objects
    if existing:
        max_bottom = max(ex['y'] + ex['height'] for ex in existing)
        return {
            'position': {
                'x': DEFAULT_MARGIN,
                'y': min(max_bottom + DEFAULT_SPACING, c_height - obj_h),
                'width': obj_w,
                'height': obj_h,
            },
            'reasoning': '기존 오브젝트 아래에 배치 (빈 공간 부족)',
        }

    return {
        'position': {'x': DEFAULT_MARGIN, 'y': DEFAULT_MARGIN, 'width': obj_w, 'height': obj_h},
        'reasoning': '기본 위치 (좌측 상단)',
    }


# ---------------------------------------------------------------------------
# Registration
# ---------------------------------------------------------------------------

def register(router):
    """Register layout services."""
    router.register('layout/suggest', handle_layout_suggest)
    router.register('layout/optimize', handle_layout_optimize)
    router.register('layout/validate', handle_layout_validate)
    router.register('layout/smart-place', handle_layout_smart_place)
    logger.info("Layout service registered (4 endpoints)")

"""
Analysis services: trend, correlation, report.
Phase 2 — Batch pool workload.
"""

import logging
import math
import statistics
from typing import List, Optional

logger = logging.getLogger(__name__)


async def handle_analysis_trend(payload: dict) -> dict:
    """
    analysis.trend — time-series trend analysis.
    Input: {tag: str, values: list, timestamps: list}
    Output: {trend: str, slope: float, r_squared: float, ...}
    """
    payload = payload or {}
    values = payload.get('values', [])
    if not values or len(values) < 3:
        return {'trend': 'insufficient_data', 'slope': 0.0, 'r_squared': 0.0}

    n = len(values)
    x = list(range(n))
    mean_x = sum(x) / n
    mean_y = sum(values) / n

    # Linear regression
    ss_xy = sum((x[i] - mean_x) * (values[i] - mean_y) for i in range(n))
    ss_xx = sum((x[i] - mean_x) ** 2 for i in range(n))
    ss_yy = sum((values[i] - mean_y) ** 2 for i in range(n))

    slope = ss_xy / ss_xx if ss_xx != 0 else 0.0
    intercept = mean_y - slope * mean_x
    r_squared = (ss_xy ** 2) / (ss_xx * ss_yy) if ss_xx * ss_yy != 0 else 0.0

    # Trend classification
    if abs(slope) < 0.01 * abs(mean_y) if mean_y != 0 else abs(slope) < 0.01:
        trend = 'stable'
    elif slope > 0:
        trend = 'increasing'
    else:
        trend = 'decreasing'

    # Statistics
    std_dev = statistics.stdev(values) if n > 1 else 0.0
    min_val = min(values)
    max_val = max(values)

    return {
        'trend': trend,
        'slope': round(slope, 6),
        'intercept': round(intercept, 4),
        'r_squared': round(r_squared, 4),
        'mean': round(mean_y, 4),
        'std_dev': round(std_dev, 4),
        'min': min_val,
        'max': max_val,
        'count': n,
    }


async def handle_analysis_correlation(payload: dict) -> dict:
    """
    analysis.correlation — Pearson correlation between two tag series.
    Input: {values_x: list, values_y: list}
    Output: {correlation: float, strength: str}
    """
    payload = payload or {}
    x = payload.get('values_x', [])
    y = payload.get('values_y', [])

    if not x or not y:
        return {'correlation': 0.0, 'strength': 'insufficient_data'}

    n = min(len(x), len(y))
    if n < 3:
        return {'correlation': 0.0, 'strength': 'insufficient_data'}

    x = x[:n]
    y = y[:n]

    mean_x = sum(x) / n
    mean_y = sum(y) / n

    cov_xy = sum((x[i] - mean_x) * (y[i] - mean_y) for i in range(n)) / n
    std_x = math.sqrt(sum((v - mean_x) ** 2 for v in x) / n)
    std_y = math.sqrt(sum((v - mean_y) ** 2 for v in y) / n)

    if std_x == 0 or std_y == 0:
        r = 0.0
    else:
        r = cov_xy / (std_x * std_y)

    # Strength classification
    abs_r = abs(r)
    if abs_r >= 0.8:
        strength = 'very_strong'
    elif abs_r >= 0.6:
        strength = 'strong'
    elif abs_r >= 0.4:
        strength = 'moderate'
    elif abs_r >= 0.2:
        strength = 'weak'
    else:
        strength = 'negligible'

    return {
        'correlation': round(r, 4),
        'strength': strength,
        'direction': 'positive' if r > 0 else 'negative' if r < 0 else 'none',
        'count': n,
    }


async def handle_analysis_report(payload: dict) -> dict:
    """
    analysis.report — generate summary report for multiple tags.
    Input: {tags: [{name, values}], period: str}
    Output: {summary: [...], generated_at: str}
    """
    import datetime
    payload = payload or {}
    tags = payload.get('tags', [])

    summaries = []
    for tag_data in tags:
        name = tag_data.get('name', 'unknown')
        values = tag_data.get('values', [])
        if values:
            summaries.append({
                'tag': name,
                'count': len(values),
                'mean': round(sum(values) / len(values), 4),
                'min': min(values),
                'max': max(values),
                'std_dev': round(statistics.stdev(values), 4) if len(values) > 1 else 0,
            })
        else:
            summaries.append({'tag': name, 'count': 0})

    return {
        'summary': summaries,
        'period': payload.get('period', 'unknown'),
        'generated_at': datetime.datetime.now().isoformat(),
        'tag_count': len(tags),
    }


def register(router):
    router.register('analysis/trend', handle_analysis_trend)
    router.register('analysis/correlation', handle_analysis_correlation)
    router.register('analysis/report', handle_analysis_report)

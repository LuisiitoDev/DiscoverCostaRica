#!/usr/bin/env bash
# Fails when merged line or branch coverage is below the threshold.
# Usage: eng/check-coverage.sh <merged Cobertura.xml> <minimum percent>
set -euo pipefail

report="${1:?path to merged Cobertura.xml}"
minimum="${2:-80}"

rate() { grep -o -m1 "$1=\"[0-9.]*\"" "$report" | head -1 | cut -d'"' -f2; }
line=$(awk -v r="$(rate line-rate)" 'BEGIN { printf "%.1f", r * 100 }')
branch=$(awk -v r="$(rate branch-rate)" 'BEGIN { printf "%.1f", r * 100 }')

echo "Line coverage:   ${line}% (minimum ${minimum}%)"
echo "Branch coverage: ${branch}% (minimum ${minimum}%)"

if awk -v l="$line" -v b="$branch" -v m="$minimum" 'BEGIN { exit !(l < m || b < m) }'; then
  echo "::error::Coverage is below ${minimum}%."
  exit 1
fi

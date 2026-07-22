# Analysis pipeline

This directory contains the historical FisheyeVR data-analysis pipeline. Raw
experiment logs, questionnaire exports, participant identifiers, generated
tables, and notebook outputs are intentionally excluded from the public tree.

## Studies and notebooks

### `CHI24/`

| Purpose | Notebook or script |
| --- | --- |
| Parse one formative-study log | `report_formal.py` |
| Parse one summative-game log | `report_game.py` |
| Per-participant reports | `singleUserReport_formal.ipynb`, `singleUserReport_game.ipynb` |
| Aggregate conditions | `combination_formal.ipynb`, `combination_game.ipynb` |
| Multi-participant summaries | `dedicated_formal.ipynb`, `dedicated_game.ipynb` |
| Questionnaire processing | `question.ipynb` |
| Statistical tests | `statistics_formal.ipynb`, `statistics_game.ipynb` |

`dedicated_pos_game.ipynb` contains the position-focused game analysis.
The three retained coordinate-reference maps are documented in
[`CHI24/Assets/README.md`](CHI24/Assets/README.md).

### `UIST24/`

The `combination_pilot`, `dedicated_pilot`, `singleUserReport_pilot`, and
`statistics_pilot` notebooks contain the pilot-study analysis stages.

## Environment

The snapshot was developed with Python 3.9. The original dependency file did
not pin versions, so use a disposable virtual environment:

```bash
python3 -m venv .venv
source .venv/bin/activate
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
```

Run notebooks from their study directory so relative paths resolve as shown.
All committed notebooks have empty outputs and `execution_count: null`.

## Data handling

Read [`data/README.md`](data/README.md) before supplying inputs. Data must be
pseudonymized before it reaches this repository. Use participant IDs such as
`P001`; do not use names, email addresses, recruitment identifiers, or a
re-identification key in filenames or file contents.

Generated CSV, JSON, text reports, charts, and executed notebook state should
remain local unless they have passed a separate privacy and publication review.

## Validation limits

The notebooks were structurally validated after their outputs and identifiers
were removed. They were not executed end-to-end because the private source data
and original fully pinned environment are unavailable in this public snapshot.

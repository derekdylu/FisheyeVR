# Private data staging area

This directory is for local, authorized, pseudonymized research inputs. Its
contents are ignored by Git; only this file may be committed.

The retained notebooks expect a layout similar to:

```text
data/
├── chi24/
│   ├── formal/
│   │   └── P001/
│   │       └── LOG_Formal_P001_<condition>_0.txt
│   ├── summary/
│   │   └── LOG_Sum_P001_<condition>.txt
│   ├── response_formal.json
│   └── response_summary.json
└── uist24/
    └── pilot/
```

Before placing data here:

1. Confirm that the intended use and storage are permitted by the applicable
   consent, ethics, and institutional requirements.
2. Replace direct and indirect identifiers with stable participant IDs.
3. Store any re-identification key outside this repository and outside shared
   analysis output.
4. Review generated tables, figures, and notebook outputs for small-cell or
   free-text disclosure before sharing them.

Never force-add data from this directory to Git.

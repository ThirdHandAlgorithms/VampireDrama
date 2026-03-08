# Development

## Requirements:

* Unity 6.3 LTS (6000.3.10f1)


# Maps

* You can edit maps in `Assets/Resources` with a text editor or with REXPaint
* The possible characters to use for maps are listed in `Assets/Scripts/Map/ConstructionTypes.cs`
* If you use REXPaint, use CTRL+T to re-export to .txt files

## Editing Unity Scene/Prefab Files (Python)

Unity `.unity`, `.prefab`, and `.asset` files are text-based YAML with custom `!u!` tags. They can be parsed and modified programmatically using the [`unityparser`](https://pypi.org/project/unityparser/) Python package.

### Setup

```bash
uv run --with unityparser python3 your_script.py
```

### Basic Usage

```python
from unityparser import UnityDocument

doc = UnityDocument.load_yaml("Assets/Scenes/MainScene.unity")

# Each entry is a Unity object (GameObject, MonoBehaviour, RectTransform, etc.)
for entry in doc.entries:
    print(entry.__class__.__name__, getattr(entry, "m_Name", ""))

# Look up entries by anchor (fileID)
by_anchor = {entry.anchor: entry for entry in doc.entries}
some_entry = by_anchor["829693247"]

# Modify and save
some_entry.m_Name = "NewName"
doc.dump_yaml()
```

### Adding New Entries

Clone existing entries with `copy.deepcopy()`, set a new string anchor, modify fields, and append to `doc.data`:

```python
import copy
new_entry = copy.deepcopy(template_entry)
new_entry.anchor = "900100000"  # must be a unique string
new_entry.m_Name = "MyObject"
doc.data.append(new_entry)
doc.dump_yaml()
```

### Inline Dicts (Flow Style)

Unity YAML uses inline `{key: value}` dicts for references and vectors. Use `OrderedFlowDict`:

```python
from unityparser.constants import OrderedFlowDict
ref = OrderedFlowDict([("fileID", "12345")])
vec = OrderedFlowDict([("x", "0"), ("y", "1"), ("z", "0")])
```

### Known Limitations

- `unityparser` may alter quoting on text fields (e.g. `'Blood:'` may become `0`). Always verify the diff after modifying scene files.
- Anchors must be strings, not integers.
- The tool script `tools/add_ability_ui.py` is a working example of adding UI elements to a scene programmatically.

### Script GUIDs

Each C# script has a GUID in its `.meta` file. MonoBehaviour components reference scripts by GUID:

```python
# Find a script's GUID
# cat Assets/Scripts/Game/AbilityUI.cs.meta -> guid: 2f40ddc73e34fe33e8d9ace574ce4af0

# Reference it in a MonoBehaviour entry
entry.m_Script = OrderedFlowDict([("fileID", "11500000"), ("guid", "2f40ddc73e34fe33e8d9ace574ce4af0"), ("type", "3")])
```

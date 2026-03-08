#!/usr/bin/env python3
"""Add AbilityIconSet component to GameManager in MainScene, with sprite references."""

import copy
from unityparser import UnityDocument
from unityparser.constants import OrderedFlowDict

SCENE_PATH = "Assets/Scenes/MainScene.unity"

# GUIDs
ABILITY_ICON_SET_GUID = "a07b7ea3dd0eddb18928dbc7b6a75f84"
GLAMOUR_SPRITE_GUID = "34805f69b368dbc3c97b5a7e82e46bd5"
GLAMOUR_SPRITE_FILE_ID = "3093879478420113611"

# GameManager GO anchor
GM_GO_ANCHOR = "931804142"

# New component fileID
ICON_SET_COMPONENT_ID = "900200000"


def flow(*pairs):
    return OrderedFlowDict(pairs)


def main():
    doc = UnityDocument.load_yaml(SCENE_PATH)

    by_anchor = {}
    for entry in doc.entries:
        by_anchor[entry.anchor] = entry

    gm_go = by_anchor[GM_GO_ANCHOR]

    # Find an existing MonoBehaviour to clone as template
    template_mb = None
    for entry in doc.entries:
        if entry.__class__.__name__ == "MonoBehaviour" and hasattr(entry, "m_Enabled"):
            template_mb = entry
            break

    # Create AbilityIconSet MonoBehaviour
    mb = copy.deepcopy(template_mb)
    mb.anchor = ICON_SET_COMPONENT_ID

    # Set standard fields
    mb.m_ObjectHideFlags = 0
    mb.m_CorrespondingSourceObject = flow(("fileID", "0"))
    mb.m_PrefabInstance = flow(("fileID", "0"))
    mb.m_PrefabAsset = flow(("fileID", "0"))
    mb.m_GameObject = flow(("fileID", GM_GO_ANCHOR))
    mb.m_Enabled = 1
    mb.m_EditorHideFlags = 0
    mb.m_Script = flow(("fileID", "11500000"), ("guid", ABILITY_ICON_SET_GUID), ("type", "3"))
    mb.m_Name = ""
    mb.m_EditorClassIdentifier = ""

    # Remove any cloned fields that don't belong
    keep = {
        "m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance",
        "m_PrefabAsset", "m_GameObject", "m_Enabled", "m_EditorHideFlags",
        "m_Script", "m_Name", "m_EditorClassIdentifier",
    }
    for attr in list(mb.get_attrs()):
        if attr not in keep and attr not in ("anchor", "extra_anchor_data"):
            try:
                delattr(mb, attr)
            except AttributeError:
                pass

    # Set Icons array - AbilityIconEntry structs with AbilityName + Icon
    glamour_sprite_ref = flow(
        ("fileID", GLAMOUR_SPRITE_FILE_ID),
        ("guid", GLAMOUR_SPRITE_GUID),
        ("type", "3"),
    )
    mb.Icons = [
        flow(
            ("AbilityName", "Glamour"),
            ("Icon", glamour_sprite_ref),
        ),
    ]

    # Add component reference to GameManager GO
    gm_go.m_Component.append(
        flow(("component", flow(("fileID", ICON_SET_COMPONENT_ID))))
    )

    # Add entry to document
    doc.data.append(mb)
    doc.dump_yaml()

    print(f"Added AbilityIconSet to GameManager with {len(mb.Icons)} icon(s)")


if __name__ == "__main__":
    main()

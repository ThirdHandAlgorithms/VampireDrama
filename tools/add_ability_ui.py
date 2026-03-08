#!/usr/bin/env python3
"""Add AbilityUI panel to MainScene.unity Canvas."""

import copy
from unityparser import UnityDocument
from unityparser.constants import OrderedFlowDict

SCENE_PATH = "Assets/Scenes/MainScene.unity"

# Known GUIDs
ABILITY_UI_GUID = "2f40ddc73e34fe33e8d9ace574ce4af0"
TEXT_GUID = "5f7201a12d95ffc409449d95f23cf332"  # UnityEngine.UI.Text
IMAGE_GUID = "fe87c0e1cc204ed48ad3b37840f39efc"  # UnityEngine.UI.Image

# Canvas RectTransform fileID
CANVAS_RT_ID = "829693247"

# Font reference (reuse from existing labels)
FONT_REF = OrderedFlowDict([("fileID", "12800000"), ("guid", "773f0e8f3d28cc3409e815285cdabdfa"), ("type", "3")])

# fileIDs - pick high unique numbers unlikely to collide
BASE_ID = 900100000


def flow(*pairs):
    return OrderedFlowDict(pairs)


def fref(file_id):
    return flow(("fileID", str(file_id)))


def zero_ref():
    return fref(0)


def script_ref(guid):
    return flow(("fileID", "11500000"), ("guid", guid), ("type", "3"))


def find_by_type_and_anchor(doc, type_name, anchor_str):
    for entry in doc.entries:
        if entry.__class__.__name__ == type_name and entry.anchor == anchor_str:
            return entry
    return None


def find_by_type_and_name(doc, type_name, name):
    for entry in doc.entries:
        if entry.__class__.__name__ == type_name and getattr(entry, "m_Name", None) == name:
            return entry
    return None


def clone_entry(template, new_anchor):
    """Deep copy an entry and set a new anchor."""
    entry = copy.deepcopy(template)
    entry.anchor = str(new_anchor)
    return entry


def set_attrs(entry, **kwargs):
    """Set multiple attributes on an entry."""
    for k, v in kwargs.items():
        setattr(entry, k, v)
    return entry


def make_go(doc, template_go, anchor, name, component_ids):
    """Create a GameObject entry by cloning a template."""
    go = clone_entry(template_go, anchor)
    go.m_Name = name
    go.m_Component = [flow(("component", fref(cid))) for cid in component_ids]
    return go


def make_rt(doc, template_rt, anchor, go_id, parent_id, children_ids,
            anchor_min=(0, 0), anchor_max=(0, 0), anchored_pos=(0, 0),
            size_delta=(100, 30), pivot=(0.5, 0.5), local_scale=(1, 1, 1)):
    """Create a RectTransform entry by cloning a template."""
    rt = clone_entry(template_rt, anchor)
    rt.m_GameObject = fref(go_id)
    rt.m_Father = fref(parent_id)
    rt.m_Children = [fref(cid) for cid in children_ids]
    rt.m_LocalScale = flow(("x", str(local_scale[0])), ("y", str(local_scale[1])), ("z", str(local_scale[2])))
    rt.m_AnchorMin = flow(("x", str(anchor_min[0])), ("y", str(anchor_min[1])))
    rt.m_AnchorMax = flow(("x", str(anchor_max[0])), ("y", str(anchor_max[1])))
    rt.m_AnchoredPosition = flow(("x", str(anchored_pos[0])), ("y", str(anchored_pos[1])))
    rt.m_SizeDelta = flow(("x", str(size_delta[0])), ("y", str(size_delta[1])))
    rt.m_Pivot = flow(("x", str(pivot[0])), ("y", str(pivot[1])))
    rt.m_LocalRotation = flow(("x", "0"), ("y", "0"), ("z", "0"), ("w", "1"))
    rt.m_LocalPosition = flow(("x", "0"), ("y", "0"), ("z", "0"))
    rt.m_LocalEulerAnglesHint = flow(("x", "0"), ("y", "0"), ("z", "0"))
    rt.m_CorrespondingSourceObject = zero_ref()
    rt.m_PrefabInstance = zero_ref()
    rt.m_PrefabAsset = zero_ref()
    return rt


def make_cr(doc, template_cr, anchor, go_id):
    """Create a CanvasRenderer entry by cloning a template."""
    cr = clone_entry(template_cr, anchor)
    cr.m_GameObject = fref(go_id)
    cr.m_CorrespondingSourceObject = zero_ref()
    cr.m_PrefabInstance = zero_ref()
    cr.m_PrefabAsset = zero_ref()
    return cr


def make_text_mono(doc, template_text, anchor, go_id, text, font_size=14, alignment=4, color=None):
    """Create a Text MonoBehaviour by cloning."""
    if color is None:
        color = flow(("r", "1"), ("g", "1"), ("b", "1"), ("a", "1"))
    mb = clone_entry(template_text, anchor)
    mb.m_GameObject = fref(go_id)
    mb.m_Script = script_ref(TEXT_GUID)
    mb.m_Color = color
    mb.m_RaycastTarget = 0
    mb.m_Text = text
    mb.m_Name = ""
    mb.m_EditorClassIdentifier = ""
    mb.m_CorrespondingSourceObject = zero_ref()
    mb.m_PrefabInstance = zero_ref()
    mb.m_PrefabAsset = zero_ref()
    # Update font data
    mb.m_FontData = OrderedFlowDict([
        ("m_Font", FONT_REF),
        ("m_FontSize", str(font_size)),
        ("m_FontStyle", "0"),
        ("m_BestFit", "0"),
        ("m_MinSize", "10"),
        ("m_MaxSize", "40"),
        ("m_Alignment", str(alignment)),
        ("m_AlignByGeometry", "0"),
        ("m_RichText", "1"),
        ("m_HorizontalOverflow", "0"),
        ("m_VerticalOverflow", "0"),
        ("m_LineSpacing", "1"),
    ])
    return mb


def make_image_mono(doc, template_img, anchor, go_id, color=None):
    """Create an Image MonoBehaviour by cloning."""
    if color is None:
        color = flow(("r", "0"), ("g", "0"), ("b", "0"), ("a", "0.5"))
    mb = clone_entry(template_img, anchor)
    mb.m_GameObject = fref(go_id)
    mb.m_Script = script_ref(IMAGE_GUID)
    mb.m_Color = color
    mb.m_RaycastTarget = 0
    mb.m_Sprite = zero_ref()
    mb.m_Name = ""
    mb.m_EditorClassIdentifier = ""
    mb.m_CorrespondingSourceObject = zero_ref()
    mb.m_PrefabInstance = zero_ref()
    mb.m_PrefabAsset = zero_ref()
    return mb


def make_ability_ui_mono(doc, template_mb, anchor, go_id, name_text_id, status_text_id, icon_id, cooldown_id):
    """Create AbilityUI MonoBehaviour by cloning."""
    mb = clone_entry(template_mb, anchor)
    mb.m_GameObject = fref(go_id)
    mb.m_Script = script_ref(ABILITY_UI_GUID)
    mb.m_Name = ""
    mb.m_EditorClassIdentifier = ""
    mb.m_CorrespondingSourceObject = zero_ref()
    mb.m_PrefabInstance = zero_ref()
    mb.m_PrefabAsset = zero_ref()
    mb.m_Enabled = 1
    # Remove cloned fields that don't belong to AbilityUI
    for attr in list(mb.get_attrs()):
        if attr.startswith("m_") and attr not in (
            "m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance",
            "m_PrefabAsset", "m_GameObject", "m_Enabled", "m_EditorHideFlags",
            "m_Script", "m_Name", "m_EditorClassIdentifier",
        ):
            try:
                delattr(mb, attr)
            except AttributeError:
                pass
    # Set AbilityUI specific fields
    mb.AbilityNameText = fref(name_text_id)
    mb.AbilityStatusText = fref(status_text_id)
    mb.AbilityIcon = fref(icon_id)
    mb.CooldownOverlay = fref(cooldown_id)
    return mb


def main():
    doc = UnityDocument.load_yaml(SCENE_PATH)

    # Find template entries to clone from
    by_anchor = {}
    for entry in doc.entries:
        by_anchor[entry.anchor] = entry

    # Templates: use BloodLabel (a Text GO with RT, CR, Text MonoBehaviour)
    blood_label_go = find_by_type_and_name(doc, "GameObject", "BloodLabel")
    # Get its components
    bl_comps = blood_label_go.m_Component
    bl_rt = by_anchor[bl_comps[0]["component"]["fileID"]]   # RectTransform
    bl_cr = by_anchor[bl_comps[1]["component"]["fileID"]]   # CanvasRenderer
    bl_text = by_anchor[bl_comps[2]["component"]["fileID"]] # Text MonoBehaviour

    # Image template: from ItemSlotGrid (has an Image component)
    isg_go = find_by_type_and_name(doc, "GameObject", "ItemSlotGrid")
    isg_comps = isg_go.m_Component
    isg_img = by_anchor[isg_comps[2]["component"]["fileID"]]  # Image MonoBehaviour

    # Canvas RT to add our panel as child
    canvas_rt = by_anchor[CANVAS_RT_ID]

    # Allocate fileIDs
    panel_go_id     = BASE_ID + 0
    panel_rt_id     = BASE_ID + 1
    panel_cr_id     = BASE_ID + 2
    panel_img_id    = BASE_ID + 3
    panel_ui_id     = BASE_ID + 4
    name_go_id      = BASE_ID + 10
    name_rt_id      = BASE_ID + 11
    name_cr_id      = BASE_ID + 12
    name_text_id    = BASE_ID + 13
    status_go_id    = BASE_ID + 20
    status_rt_id    = BASE_ID + 21
    status_cr_id    = BASE_ID + 22
    status_text_id  = BASE_ID + 23
    icon_go_id      = BASE_ID + 30
    icon_rt_id      = BASE_ID + 31
    icon_cr_id      = BASE_ID + 32
    icon_img_id     = BASE_ID + 33
    cd_go_id        = BASE_ID + 40
    cd_rt_id        = BASE_ID + 41
    cd_cr_id        = BASE_ID + 42
    cd_img_id       = BASE_ID + 43

    new_entries = []

    # --- AbilityPanel (GO + RT + CR + Image + AbilityUI) ---
    new_entries.append(make_go(doc, blood_label_go, panel_go_id, "AbilityPanel",
                               [panel_rt_id, panel_cr_id, panel_img_id, panel_ui_id]))
    new_entries.append(make_rt(doc, bl_rt, panel_rt_id, panel_go_id, int(CANVAS_RT_ID),
                               [name_rt_id, status_rt_id, icon_rt_id],
                               anchor_min=(0, 1), anchor_max=(0, 1),
                               anchored_pos=(45, -38), size_delta=(80, 25),
                               local_scale=(3, 3, 1)))
    new_entries.append(make_cr(doc, bl_cr, panel_cr_id, panel_go_id))
    new_entries.append(make_image_mono(doc, isg_img, panel_img_id, panel_go_id,
                                       color=flow(("r", "0"), ("g", "0"), ("b", "0"), ("a", "0.4"))))
    new_entries.append(make_ability_ui_mono(doc, bl_text, panel_ui_id, panel_go_id,
                                            name_text_id, status_text_id, icon_img_id, cd_img_id))

    # --- AbilityNameText (GO + RT + CR + Text) ---
    new_entries.append(make_go(doc, blood_label_go, name_go_id, "AbilityNameText",
                               [name_rt_id, name_cr_id, name_text_id]))
    new_entries.append(make_rt(doc, bl_rt, name_rt_id, name_go_id, panel_rt_id, [],
                               anchor_min=(0, 1), anchor_max=(1, 1),
                               anchored_pos=(0, -3), size_delta=(0, 8),
                               pivot=(0.5, 1), local_scale=(0.333, 0.333, 1)))
    new_entries.append(make_cr(doc, bl_cr, name_cr_id, name_go_id))
    new_entries.append(make_text_mono(doc, bl_text, name_text_id, name_go_id, "Ability",
                                      font_size=14, alignment=4,
                                      color=flow(("r", "1"), ("g", "0.85"), ("b", "0.2"), ("a", "1"))))

    # --- AbilityStatusText (GO + RT + CR + Text) ---
    new_entries.append(make_go(doc, blood_label_go, status_go_id, "AbilityStatusText",
                               [status_rt_id, status_cr_id, status_text_id]))
    new_entries.append(make_rt(doc, bl_rt, status_rt_id, status_go_id, panel_rt_id, [],
                               anchor_min=(0, 0), anchor_max=(1, 0),
                               anchored_pos=(0, 3), size_delta=(0, 8),
                               pivot=(0.5, 0), local_scale=(0.333, 0.333, 1)))
    new_entries.append(make_cr(doc, bl_cr, status_cr_id, status_go_id))
    new_entries.append(make_text_mono(doc, bl_text, status_text_id, status_go_id, "Ready",
                                      font_size=14, alignment=4,
                                      color=flow(("r", "0.7"), ("g", "0.7"), ("b", "0.7"), ("a", "1"))))

    # --- AbilityIcon (GO + RT + CR + Image) ---
    new_entries.append(make_go(doc, blood_label_go, icon_go_id, "AbilityIcon",
                               [icon_rt_id, icon_cr_id, icon_img_id]))
    new_entries.append(make_rt(doc, bl_rt, icon_rt_id, icon_go_id, panel_rt_id, [cd_rt_id],
                               anchor_min=(0.5, 0.5), anchor_max=(0.5, 0.5),
                               anchored_pos=(0, 0), size_delta=(10, 10),
                               local_scale=(0.333, 0.333, 1)))
    new_entries.append(make_cr(doc, bl_cr, icon_cr_id, icon_go_id))
    new_entries.append(make_image_mono(doc, isg_img, icon_img_id, icon_go_id,
                                       color=flow(("r", "0.8"), ("g", "0.2"), ("b", "0.2"), ("a", "1"))))

    # --- CooldownOverlay (GO + RT + CR + Image) ---
    new_entries.append(make_go(doc, blood_label_go, cd_go_id, "CooldownOverlay",
                               [cd_rt_id, cd_cr_id, cd_img_id]))
    new_entries.append(make_rt(doc, bl_rt, cd_rt_id, cd_go_id, icon_rt_id, [],
                               anchor_min=(0, 0), anchor_max=(1, 1),
                               anchored_pos=(0, 0), size_delta=(0, 0)))
    new_entries.append(make_cr(doc, bl_cr, cd_cr_id, cd_go_id))
    new_entries.append(make_image_mono(doc, isg_img, cd_img_id, cd_go_id,
                                       color=flow(("r", "0"), ("g", "0"), ("b", "0"), ("a", "0.6"))))

    # Add panel as child of Canvas RT
    canvas_rt.m_Children.append(fref(panel_rt_id))

    # Add all entries to doc
    for entry in new_entries:
        doc.data.append(entry)

    # Dump back
    doc.dump_yaml()

    print(f"Added {len(new_entries)} entries to {SCENE_PATH}")
    print("AbilityPanel positioned top-left with name, status, icon, and cooldown overlay")


if __name__ == "__main__":
    main()

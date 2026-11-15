import bpy
import math
import os

def clear_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)

def create_material(name, color, roughness=0.5, metallic=0.0):
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = roughness
        bsdf.inputs["Metallic"].default_value = metallic
    return mat

def assign_material(obj, mat):
    if obj.data.materials:
        obj.data.materials[0] = mat
    else:
        obj.data.materials.append(mat)

# ----------------------------------------------------
# 1. KHÁCH HÀNG NAM (Customer_Male)
# ----------------------------------------------------
def build_male_customer():
    clear_scene()
    
    mat_skin = create_material("Mat_MaleSkin", (0.85, 0.70, 0.58, 1.0), roughness=0.6)
    mat_hair = create_material("Mat_MaleHair", (0.15, 0.10, 0.08, 1.0), roughness=0.8)
    mat_coat = create_material("Mat_MaleCoat", (0.20, 0.28, 0.38, 1.0), roughness=0.5)
    mat_pants = create_material("Mat_MalePants", (0.12, 0.13, 0.15, 1.0), roughness=0.6)
    mat_shoes = create_material("Mat_MaleShoes", (0.05, 0.05, 0.05, 1.0), roughness=0.3)

    parts = []

    # Torso (Áo khoác)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.15), scale=(0.28, 0.18, 0.40))
    torso = bpy.context.active_object
    torso.name = "Torso"
    assign_material(torso, mat_coat)
    parts.append(torso)

    # Head (Đầu)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, 0, 1.72), radius=0.14)
    head = bpy.context.active_object
    head.name = "Head"
    assign_material(head, mat_skin)
    parts.append(head)

    # Hair (Tóc ngắn)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, -0.02, 1.76), radius=0.145)
    hair = bpy.context.active_object
    hair.name = "Hair"
    hair.scale = (1.02, 1.05, 0.8)
    assign_material(hair, mat_hair)
    parts.append(hair)

    # Legs (Quần tây)
    for side, x in [("L", -0.12), ("R", 0.12)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.08, depth=0.65, location=(x, 0, 0.45))
        leg = bpy.context.active_object
        leg.name = f"Leg_{side}"
        assign_material(leg, mat_pants)
        parts.append(leg)

        # Shoes (Giày da)
        bpy.ops.mesh.primitive_cube_add(location=(x, 0.05, 0.06), scale=(0.07, 0.14, 0.06))
        shoe = bpy.context.active_object
        shoe.name = f"Shoe_{side}"
        assign_material(shoe, mat_shoes)
        parts.append(shoe)

    # Arms (Tay áo khoác)
    for side, x in [("L", -0.36), ("R", 0.36)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.065, depth=0.55, location=(x, 0, 1.10))
        arm = bpy.context.active_object
        arm.name = f"Arm_{side}"
        assign_material(arm, mat_coat)
        parts.append(arm)

        # Hands (Bàn tay)
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, 0, 0.78), radius=0.055)
        hand = bpy.context.active_object
        hand.name = f"Hand_{side}"
        assign_material(hand, mat_skin)
        parts.append(hand)

    # Join into single object
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Male"
    return torso

# ----------------------------------------------------
# 2. KHÁCH HÀNG NỮ (Customer_Female - Sarah Jenkins)
# ----------------------------------------------------
def build_female_customer():
    clear_scene()

    mat_skin = create_material("Mat_FemSkin", (0.92, 0.76, 0.65, 1.0), roughness=0.5)
    mat_hair = create_material("Mat_FemHair", (0.45, 0.25, 0.12, 1.0), roughness=0.7)
    mat_hoodie = create_material("Mat_FemHoodie", (0.75, 0.30, 0.35, 1.0), roughness=0.7)
    mat_jeans = create_material("Mat_FemJeans", (0.15, 0.22, 0.40, 1.0), roughness=0.6)
    mat_boots = create_material("Mat_FemBoots", (0.25, 0.15, 0.10, 1.0), roughness=0.4)

    parts = []

    # Torso (Áo Hoodie)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.08), scale=(0.24, 0.16, 0.36))
    torso = bpy.context.active_object
    torso.name = "Torso"
    assign_material(torso, mat_hoodie)
    parts.append(torso)

    # Head (Đầu)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, 0, 1.58), radius=0.13)
    head = bpy.context.active_object
    head.name = "Head"
    assign_material(head, mat_skin)
    parts.append(head)

    # Hair (Tóc buộc đuôi ngựa)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, -0.02, 1.62), radius=0.135)
    hair = bpy.context.active_object
    hair.name = "Hair_Base"
    hair.scale = (1.02, 1.06, 0.85)
    assign_material(hair, mat_hair)
    parts.append(hair)

    # Ponytail (Đuôi tóc dài phía sau)
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.045, depth=0.28, location=(0, -0.16, 1.48))
    pony = bpy.context.active_object
    pony.name = "Ponytail"
    pony.rotation_euler = (math.radians(20), 0, 0)
    assign_material(pony, mat_hair)
    parts.append(pony)

    # Legs (Quần Jeans)
    for side, x in [("L", -0.11), ("R", 0.11)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.07, depth=0.62, location=(x, 0, 0.42))
        leg = bpy.context.active_object
        leg.name = f"Leg_{side}"
        assign_material(leg, mat_jeans)
        parts.append(leg)

        # Boots (Bốt da)
        bpy.ops.mesh.primitive_cube_add(location=(x, 0.04, 0.06), scale=(0.065, 0.12, 0.06))
        boot = bpy.context.active_object
        boot.name = f"Boot_{side}"
        assign_material(boot, mat_boots)
        parts.append(boot)

    # Arms
    for side, x in [("L", -0.32), ("R", 0.32)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.055, depth=0.50, location=(x, 0, 1.02))
        arm = bpy.context.active_object
        arm.name = f"Arm_{side}"
        assign_material(arm, mat_hoodie)
        parts.append(arm)

        # Hands
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, 0, 0.72), radius=0.048)
        hand = bpy.context.active_object
        hand.name = f"Hand_{side}"
        assign_material(hand, mat_skin)
        parts.append(hand)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Female"
    return torso

# ----------------------------------------------------
# 3. KẺ GIẢ MẠO (Customer_Doppelganger - Thomas Vance)
# ----------------------------------------------------
def build_doppelganger():
    clear_scene()

    mat_skin = create_material("Mat_DopSkin", (0.42, 0.44, 0.40, 1.0), roughness=0.4) # Da xám tro tái nhợt
    mat_coat = create_material("Mat_DopCoat", (0.10, 0.10, 0.11, 1.0), roughness=0.9) # Áo măng tô rách đen
    mat_eyes = create_material("Mat_DopEyes", (0.9, 0.1, 0.05, 1.0), roughness=0.1)  # Mắt đỏ ngầu dị thường

    parts = []

    # Torso (Thân gù, vặn vẹo)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0.05, 1.15), scale=(0.30, 0.20, 0.44))
    torso = bpy.context.active_object
    torso.name = "Torso"
    torso.rotation_euler = (math.radians(12), 0, math.radians(-5)) # Gù và lệch vai
    assign_material(torso, mat_coat)
    parts.append(torso)

    # Twisted Neck & Head (Cổ ngoẹo 45 độ, đầu méo mó)
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.07, depth=0.18, location=(0.06, 0.08, 1.58))
    neck = bpy.context.active_object
    neck.name = "Neck_Twisted"
    neck.rotation_euler = (0, math.radians(35), math.radians(25))
    assign_material(neck, mat_skin)
    parts.append(neck)

    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0.14, 0.08, 1.74), radius=0.15)
    head = bpy.context.active_object
    head.name = "Head_Doppelganger"
    head.scale = (0.85, 1.25, 0.95) # Đầu dài bất thường
    head.rotation_euler = (math.radians(-15), math.radians(35), math.radians(30))
    assign_material(head, mat_skin)
    parts.append(head)

    # Eyes (Mắt đỏ dị thường)
    for eye_x, eye_z in [(-0.04, 0.03), (0.05, 0.04)]:
        bpy.ops.mesh.primitive_uv_sphere_add(segments=8, ring_count=8, location=(0.14 + eye_x, 0.21, 1.76 + eye_z), radius=0.025)
        eye = bpy.context.active_object
        assign_material(eye, mat_eyes)
        parts.append(eye)

    # Elongated Arms (Cánh tay dài thượt bất thường chạm gối)
    for side, x, rot in [("L", -0.38, 8), ("R", 0.40, -14)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.055, depth=0.75, location=(x, 0.04, 0.95))
        arm = bpy.context.active_object
        arm.name = f"Arm_Long_{side}"
        arm.rotation_euler = (math.radians(rot), 0, math.radians(rot * 0.5))
        assign_material(arm, mat_coat)
        parts.append(arm)

        # Claws / Sharp Fingers (Bàn tay dài có móng nhọn)
        bpy.ops.mesh.primitive_cone_add(vertices=8, radius1=0.05, depth=0.16, location=(x, 0.04, 0.50))
        claw = bpy.context.active_object
        claw.name = f"Claw_{side}"
        claw.rotation_euler = (math.radians(180), 0, 0)
        assign_material(claw, mat_skin)
        parts.append(claw)

    # Legs (Chân khẳng khiu)
    for side, x in [("L", -0.13), ("R", 0.13)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.075, depth=0.68, location=(x, 0, 0.42))
        leg = bpy.context.active_object
        leg.name = f"Leg_{side}"
        assign_material(leg, mat_coat)
        parts.append(leg)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Doppelganger"
    return torso

# ----------------------------------------------------
# 4. CON BOT / QUÁI TRÙM (Boss_Monster - The Flesh Stalker)
# ----------------------------------------------------
def build_boss_monster():
    clear_scene()

    mat_flesh = create_material("Mat_BossFlesh", (0.28, 0.08, 0.08, 1.0), roughness=0.3) # Thịt đỏ bầm nhớp nháp
    mat_bone = create_material("Mat_BossBone", (0.85, 0.82, 0.70, 1.0), roughness=0.4)   # Gai xương nhô ra
    mat_glow = create_material("Mat_BossGlowEye", (1.0, 0.05, 0.0, 1.0), roughness=0.1) # Mắt đỏ rực phát quang

    parts = []

    # Massive Muscular Torso (Khối ngực khổng lồ, cao 2.8m)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.70), scale=(0.75, 0.45, 0.65))
    torso = bpy.context.active_object
    torso.name = "Boss_Torso"
    torso.rotation_euler = (math.radians(15), 0, 0) # Lao về phía trước
    assign_material(torso, mat_flesh)
    parts.append(torso)

    # Monstrous Head & Maw (Đầu quái vật gồ ghề với hàm răng cắn xé)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, 0.35, 2.35), radius=0.32)
    head = bpy.context.active_object
    head.name = "Boss_Head"
    head.scale = (0.9, 1.3, 0.9)
    assign_material(head, mat_flesh)
    parts.append(head)

    # Red Glowing Eyes (4 Mắt đỏ phát sáng rùng rợn)
    eye_coords = [(-0.16, 0.62, 2.45), (0.16, 0.62, 2.45), (-0.10, 0.66, 2.35), (0.10, 0.66, 2.35)]
    for idx, (ex, ey, ez) in enumerate(eye_coords):
        bpy.ops.mesh.primitive_uv_sphere_add(segments=8, ring_count=8, location=(ex, ey, ez), radius=0.045)
        eye = bpy.context.active_object
        eye.name = f"Boss_Eye_{idx+1}"
        assign_material(eye, mat_glow)
        parts.append(eye)

    # Horns / Spikes on Shoulders (Các gai xương nhọn hoắt mọc trên vai & lưng)
    for side, x in [("L", -0.55), ("R", 0.55)]:
        for i, z_off in enumerate([0.15, 0.35]):
            bpy.ops.mesh.primitive_cone_add(vertices=8, radius1=0.08, depth=0.45, location=(x * (1 + i * 0.2), -0.15, 2.15 + z_off))
            spike = bpy.context.active_object
            spike.name = f"Spike_{side}_{i}"
            spike.rotation_euler = (math.radians(-30), math.radians(-25 if side == "L" else 25), 0)
            assign_material(spike, mat_bone)
            parts.append(spike)

    # Huge Muscle Arms (Hai cánh tay quái vật to lớn, dài chạm đất)
    for side, x, sign in [("L", -0.92, -1), ("R", 0.92, 1)]:
        # Upper arm
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.20, depth=0.85, location=(x, 0.10, 1.55))
        uarm = bpy.context.active_object
        uarm.name = f"Boss_UpperArm_{side}"
        uarm.rotation_euler = (math.radians(20), math.radians(sign * 15), 0)
        assign_material(uarm, mat_flesh)
        parts.append(uarm)

        # Forearm
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.18, depth=0.90, location=(x * 1.05, 0.35, 0.85))
        farm = bpy.context.active_object
        farm.name = f"Boss_Forearm_{side}"
        farm.rotation_euler = (math.radians(-15), 0, 0)
        assign_material(farm, mat_flesh)
        parts.append(farm)

        # Razor Claws (Bộ móng vuốt xương nhọn hoắt)
        bpy.ops.mesh.primitive_cone_add(vertices=8, radius1=0.14, depth=0.35, location=(x * 1.05, 0.42, 0.30))
        claw = bpy.context.active_object
        claw.name = f"Boss_Claw_{side}"
        claw.rotation_euler = (math.radians(180), 0, 0)
        assign_material(claw, mat_bone)
        parts.append(claw)

    # Heavy Legs (Đôi chân trụ vững chắc như cột đình)
    for side, x in [("L", -0.38), ("R", 0.38)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.22, depth=1.10, location=(x, -0.05, 0.60))
        leg = bpy.context.active_object
        leg.name = f"Boss_Leg_{side}"
        assign_material(leg, mat_flesh)
        parts.append(leg)

        # Foot
        bpy.ops.mesh.primitive_cube_add(location=(x, 0.15, 0.10), scale=(0.20, 0.35, 0.12))
        foot = bpy.context.active_object
        foot.name = f"Boss_Foot_{side}"
        assign_material(foot, mat_flesh)
        parts.append(foot)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Boss_Monster"
    return torso

# ----------------------------------------------------
# MAIN EXPORT LOGIC
# ----------------------------------------------------
def main():
    output_dir = r"D:\ProjectGame\Game\Assets\Models"
    os.makedirs(output_dir, exist_ok=True)

    generators = [
        ("Customer_Male", build_male_customer),
        ("Customer_Female", build_female_customer),
        ("Customer_Doppelganger", build_doppelganger),
        ("Boss_Monster", build_boss_monster)
    ]

    all_objects = []

    for name, gen_func in generators:
        print(f"Generating model: {name}...")
        obj = gen_func()
        obj_file = os.path.join(output_dir, f"{name}.obj")
        
        # Select only this object
        bpy.ops.object.select_all(action='DESELECT')
        obj.select_set(True)
        bpy.context.view_layer.objects.active = obj
        
        # Export OBJ
        try:
            bpy.ops.wm.obj_export(filepath=obj_file, export_selected_objects=True)
        except Exception:
            try:
                bpy.ops.export_scene.obj(filepath=obj_file, use_selection=True)
            except Exception as e:
                print(f"Export OBJ failed for {name}: {e}")
        
        print(f"Exported: {obj_file}")

    # Save a combined .blend project with all 4 models arranged side-by-side
    clear_scene()
    m1 = build_male_customer()
    m1.location = (-3.0, 0, 0)
    
    m2 = build_female_customer()
    m2.location = (-1.0, 0, 0)
    
    m3 = build_doppelganger()
    m3.location = (1.0, 0, 0)
    
    m4 = build_boss_monster()
    m4.location = (4.0, 0, 0)

    blend_file = os.path.join(output_dir, "MidnightMart_Characters.blend")
    bpy.ops.wm.save_as_mainfile(filepath=blend_file)
    print(f"Saved combined Blender project to: {blend_file}")
    print("ALL 4 MODELS GENERATED SUCCESSFULLY!")

if __name__ == "__main__":
    main()


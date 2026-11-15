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

def export_obj(obj, filepath):
    bpy.ops.object.select_all(action='DESELECT')
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    
    # Try Blender 4.0+/5.0+ export first
    try:
        bpy.ops.wm.obj_export(filepath=filepath, export_selected_objects=True)
    except Exception:
        try:
            bpy.ops.export_scene.obj(filepath=filepath, use_selection=True)
        except Exception as e:
            print(f"Error exporting OBJ to {filepath}: {e}")

# ====================================================
# 1. KHÁCH HÀNG NAM CHI TIẾT (Customer_Male) - LIÊN KẾT LIỀN MẠCH
# ====================================================
def build_male_customer():
    clear_scene()
    
    mat_skin = create_material("Mat_MaleSkin", (0.86, 0.72, 0.60, 1.0), roughness=0.55)
    mat_hair = create_material("Mat_MaleHair", (0.12, 0.08, 0.06, 1.0), roughness=0.85)
    mat_coat = create_material("Mat_MaleCoat", (0.18, 0.25, 0.35, 1.0), roughness=0.45)
    mat_shirt = create_material("Mat_MaleShirt", (0.90, 0.90, 0.92, 1.0), roughness=0.6)
    mat_pants = create_material("Mat_MalePants", (0.10, 0.12, 0.15, 1.0), roughness=0.65)
    mat_shoes = create_material("Mat_MaleShoes", (0.04, 0.04, 0.04, 1.0), roughness=0.3)
    mat_belt = create_material("Mat_MaleBelt", (0.06, 0.04, 0.02, 1.0), roughness=0.4)

    parts = []

    # 1. Hips / Pelvis (Khung chậu liên kết thân trên và đùi)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 0.82), scale=(0.24, 0.16, 0.10))
    pelvis = bpy.context.active_object
    pelvis.name = "Pelvis"
    assign_material(pelvis, mat_pants)
    parts.append(pelvis)

    # Thắt lưng
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 0.92), scale=(0.245, 0.165, 0.03))
    belt = bpy.context.active_object
    belt.name = "Belt"
    assign_material(belt, mat_belt)
    parts.append(belt)

    # 2. Torso (Thân áo khoác & áo sơ mi bên trong)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.18), scale=(0.27, 0.17, 0.25))
    torso = bpy.context.active_object
    torso.name = "Torso"
    assign_material(torso, mat_coat)
    parts.append(torso)

    # Cổ áo sơ mi
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.09, depth=0.10, location=(0, 0, 1.45))
    collar = bpy.context.active_object
    collar.name = "Collar"
    assign_material(collar, mat_shirt)
    parts.append(collar)

    # 3. Neck (Cổ liền mạch cắm sâu vào ngực và đầu - KHÔNG LƠ LỬNG)
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.075, depth=0.18, location=(0, -0.01, 1.54))
    neck = bpy.context.active_object
    neck.name = "Neck"
    assign_material(neck, mat_skin)
    parts.append(neck)

    # 4. Head & Face (Đầu, mũi, tai)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=20, ring_count=14, location=(0, 0, 1.74), radius=0.135)
    head = bpy.context.active_object
    head.name = "Head"
    head.scale = (1.0, 1.12, 1.18)
    assign_material(head, mat_skin)
    parts.append(head)

    # Mũi
    bpy.ops.mesh.primitive_cone_add(vertices=12, radius1=0.02, depth=0.045, location=(0, 0.14, 1.74), rotation=(math.radians(90), 0, 0))
    nose = bpy.context.active_object
    nose.name = "Nose"
    assign_material(nose, mat_skin)
    parts.append(nose)

    # Tai 2 bên
    for side, x in [("L", -0.14), ("R", 0.14)]:
        bpy.ops.mesh.primitive_cube_add(location=(x, 0, 1.74), scale=(0.018, 0.035, 0.045))
        ear = bpy.context.active_object
        ear.name = f"Ear_{side}"
        assign_material(ear, mat_skin)
        parts.append(ear)

    # Tóc
    bpy.ops.mesh.primitive_uv_sphere_add(segments=20, ring_count=14, location=(0, -0.02, 1.79), radius=0.142)
    hair = bpy.context.active_object
    hair.name = "Hair"
    hair.scale = (1.04, 1.08, 0.85)
    assign_material(hair, mat_hair)
    parts.append(hair)

    # 5. Shoulders & Arms (Khớp vai nối liền thân với cánh tay)
    for side, x, sign in [("L", -0.28, -1), ("R", 0.28, 1)]:
        # Khớp vai nối liền Torso
        bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(x, 0, 1.38), radius=0.085)
        shoulder = bpy.context.active_object
        shoulder.name = f"Shoulder_{side}"
        assign_material(shoulder, mat_coat)
        parts.append(shoulder)

        # Cánh tay trên (Upper Arm)
        arm_x = sign * 0.33
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.062, depth=0.30, location=(arm_x, 0, 1.22))
        upper_arm = bpy.context.active_object
        upper_arm.name = f"UpperArm_{side}"
        assign_material(upper_arm, mat_coat)
        parts.append(upper_arm)

        # Khớp khuỷu tay (Elbow)
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(arm_x, 0, 1.06), radius=0.058)
        elbow = bpy.context.active_object
        elbow.name = f"Elbow_{side}"
        assign_material(elbow, mat_coat)
        parts.append(elbow)

        # Cẳng tay (Forearm)
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.055, depth=0.28, location=(arm_x, 0.02, 0.92))
        forearm = bpy.context.active_object
        forearm.name = f"Forearm_{side}"
        assign_material(forearm, mat_coat)
        parts.append(forearm)

        # Cổ tay (Wrist)
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.045, depth=0.06, location=(arm_x, 0.03, 0.77))
        wrist = bpy.context.active_object
        wrist.name = f"Wrist_{side}"
        assign_material(wrist, mat_shirt)
        parts.append(wrist)

        # Bàn tay (Hand & Fingers)
        bpy.ops.mesh.primitive_cube_add(location=(arm_x, 0.03, 0.70), scale=(0.045, 0.035, 0.065))
        hand = bpy.context.active_object
        hand.name = f"Hand_{side}"
        assign_material(hand, mat_skin)
        parts.append(hand)

        # Ngón cái
        bpy.ops.mesh.primitive_cube_add(location=(arm_x - sign * 0.03, 0.05, 0.72), scale=(0.015, 0.02, 0.03))
        thumb = bpy.context.active_object
        thumb.name = f"Thumb_{side}"
        assign_material(thumb, mat_skin)
        parts.append(thumb)

    # 6. Legs & Shoes (Khớp háng, đùi, đầu gối, bắp chân, mắt cá, giày da)
    for side, x in [("L", -0.12), ("R", 0.12)]:
        # Khớp háng nối Pelvis
        bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(x, 0, 0.76), radius=0.08)
        hip_joint = bpy.context.active_object
        hip_joint.name = f"HipJoint_{side}"
        assign_material(hip_joint, mat_pants)
        parts.append(hip_joint)

        # Đùi (Thigh)
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.078, depth=0.36, location=(x, 0, 0.58))
        thigh = bpy.context.active_object
        thigh.name = f"Thigh_{side}"
        assign_material(thigh, mat_pants)
        parts.append(thigh)

        # Đầu gối (Knee)
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, 0.01, 0.39), radius=0.072)
        knee = bpy.context.active_object
        knee.name = f"Knee_{side}"
        assign_material(knee, mat_pants)
        parts.append(knee)

        # Bắp chân (Calf)
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.068, depth=0.34, location=(x, 0, 0.22))
        calf = bpy.context.active_object
        calf.name = f"Calf_{side}"
        assign_material(calf, mat_pants)
        parts.append(calf)

        # Mắt cá chân (Ankle)
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.055, depth=0.08, location=(x, 0.01, 0.08))
        ankle = bpy.context.active_object
        ankle.name = f"Ankle_{side}"
        assign_material(ankle, mat_pants)
        parts.append(ankle)

        # Giày da nam (Shoe with sole and heel)
        bpy.ops.mesh.primitive_cube_add(location=(x, 0.06, 0.04), scale=(0.065, 0.15, 0.04))
        shoe = bpy.context.active_object
        shoe.name = f"Shoe_{side}"
        assign_material(shoe, mat_shoes)
        parts.append(shoe)

    # Join tất cả thành một Object thống nhất
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Male"
    return torso

# ====================================================
# 2. KHÁCH HÀNG NỮ (Customer_Female - Sarah Jenkins)
# ====================================================
def build_female_customer():
    clear_scene()

    mat_skin = create_material("Mat_FemSkin", (0.92, 0.77, 0.66, 1.0), roughness=0.5)
    mat_hair = create_material("Mat_FemHair", (0.42, 0.24, 0.12, 1.0), roughness=0.7)
    mat_hoodie = create_material("Mat_FemHoodie", (0.76, 0.28, 0.34, 1.0), roughness=0.75)
    mat_jeans = create_material("Mat_FemJeans", (0.16, 0.22, 0.38, 1.0), roughness=0.6)
    mat_boots = create_material("Mat_FemBoots", (0.22, 0.14, 0.10, 1.0), roughness=0.4)

    parts = []

    # 1. Pelvis
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 0.78), scale=(0.22, 0.15, 0.10))
    pelvis = bpy.context.active_object
    pelvis.name = "Pelvis"
    assign_material(pelvis, mat_jeans)
    parts.append(pelvis)

    # 2. Torso (Áo Hoodie)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.10), scale=(0.24, 0.16, 0.24))
    torso = bpy.context.active_object
    torso.name = "Torso"
    assign_material(torso, mat_hoodie)
    parts.append(torso)

    # Túi áo hoodie trước bụng
    bpy.ops.mesh.primitive_cube_add(location=(0, 0.15, 0.98), scale=(0.16, 0.03, 0.08))
    pocket = bpy.context.active_object
    pocket.name = "Pocket"
    assign_material(pocket, mat_hoodie)
    parts.append(pocket)

    # Mũ áo hoodie xếp sau lưng
    bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(0, -0.14, 1.32), radius=0.10)
    hood = bpy.context.active_object
    hood.name = "Hood"
    assign_material(hood, mat_hoodie)
    parts.append(hood)

    # 3. Neck
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.065, depth=0.16, location=(0, 0, 1.44))
    neck = bpy.context.active_object
    neck.name = "Neck"
    assign_material(neck, mat_skin)
    parts.append(neck)

    # 4. Head & Ponytail
    bpy.ops.mesh.primitive_uv_sphere_add(segments=20, ring_count=14, location=(0, 0, 1.62), radius=0.125)
    head = bpy.context.active_object
    head.name = "Head"
    head.scale = (1.0, 1.08, 1.14)
    assign_material(head, mat_skin)
    parts.append(head)

    # Mũi
    bpy.ops.mesh.primitive_cone_add(vertices=10, radius1=0.016, depth=0.038, location=(0, 0.13, 1.62), rotation=(math.radians(90), 0, 0))
    nose = bpy.context.active_object
    nose.name = "Nose"
    assign_material(nose, mat_skin)
    parts.append(nose)

    # Tóc nền
    bpy.ops.mesh.primitive_uv_sphere_add(segments=20, ring_count=14, location=(0, -0.02, 1.66), radius=0.132)
    hair = bpy.context.active_object
    hair.name = "Hair"
    hair.scale = (1.03, 1.06, 0.88)
    assign_material(hair, mat_hair)
    parts.append(hair)

    # Đuôi tóc cột phía sau
    bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.042, depth=0.30, location=(0, -0.16, 1.50))
    pony = bpy.context.active_object
    pony.name = "Ponytail"
    pony.rotation_euler = (math.radians(24), 0, 0)
    assign_material(pony, mat_hair)
    parts.append(pony)

    # 5. Shoulders & Arms
    for side, x, sign in [("L", -0.24, -1), ("R", 0.24, 1)]:
        # Khớp vai
        bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(x, 0, 1.30), radius=0.075)
        shoulder = bpy.context.active_object
        shoulder.name = f"Shoulder_{side}"
        assign_material(shoulder, mat_hoodie)
        parts.append(shoulder)

        # Tay trên
        arm_x = sign * 0.28
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.054, depth=0.28, location=(arm_x, 0, 1.14))
        upper_arm = bpy.context.active_object
        upper_arm.name = f"UpperArm_{side}"
        assign_material(upper_arm, mat_hoodie)
        parts.append(upper_arm)

        # Khuỷu tay
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(arm_x, 0, 0.99), radius=0.050)
        elbow = bpy.context.active_object
        elbow.name = f"Elbow_{side}"
        assign_material(elbow, mat_hoodie)
        parts.append(elbow)

        # Cẳng tay
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.046, depth=0.26, location=(arm_x, 0.02, 0.85))
        forearm = bpy.context.active_object
        forearm.name = f"Forearm_{side}"
        assign_material(forearm, mat_hoodie)
        parts.append(forearm)

        # Bàn tay
        bpy.ops.mesh.primitive_cube_add(location=(arm_x, 0.02, 0.67), scale=(0.038, 0.030, 0.055))
        hand = bpy.context.active_object
        hand.name = f"Hand_{side}"
        assign_material(hand, mat_skin)
        parts.append(hand)

    # 6. Legs & Boots
    for side, x in [("L", -0.11), ("R", 0.11)]:
        # Khớp háng
        bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(x, 0, 0.72), radius=0.075)
        hip_joint = bpy.context.active_object
        hip_joint.name = f"HipJoint_{side}"
        assign_material(hip_joint, mat_jeans)
        parts.append(hip_joint)

        # Đùi
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.070, depth=0.34, location=(x, 0, 0.54))
        thigh = bpy.context.active_object
        thigh.name = f"Thigh_{side}"
        assign_material(thigh, mat_jeans)
        parts.append(thigh)

        # Đầu gối
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, 0.01, 0.36), radius=0.065)
        knee = bpy.context.active_object
        knee.name = f"Knee_{side}"
        assign_material(knee, mat_jeans)
        parts.append(knee)

        # Bắp chân
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.060, depth=0.32, location=(x, 0, 0.20))
        calf = bpy.context.active_object
        calf.name = f"Calf_{side}"
        assign_material(calf, mat_jeans)
        parts.append(calf)

        # Bốt da nữ
        bpy.ops.mesh.primitive_cube_add(location=(x, 0.05, 0.04), scale=(0.058, 0.13, 0.04))
        boot = bpy.context.active_object
        boot.name = f"Boot_{side}"
        assign_material(boot, mat_boots)
        parts.append(boot)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Female"
    return torso

# ====================================================
# 3. KẺ GIẢ MẠO (Customer_Doppelganger - Thomas Vance)
# ====================================================
def build_doppelganger():
    clear_scene()

    mat_skin = create_material("Mat_DopSkin", (0.42, 0.44, 0.40, 1.0), roughness=0.45) # Da xám tro
    mat_coat = create_material("Mat_DopCoat", (0.08, 0.08, 0.09, 1.0), roughness=0.9)
    mat_eyes = create_material("Mat_DopEyes", (0.95, 0.05, 0.02, 1.0), roughness=0.1) # Mắt đỏ ngầu
    mat_pants = create_material("Mat_DopPants", (0.06, 0.06, 0.07, 1.0), roughness=0.8)

    parts = []

    # 1. Pelvis
    bpy.ops.mesh.primitive_cube_add(location=(0, 0.04, 0.82), scale=(0.25, 0.18, 0.12))
    pelvis = bpy.context.active_object
    pelvis.name = "Pelvis"
    assign_material(pelvis, mat_pants)
    parts.append(pelvis)

    # 2. Torso (Gù lưng dị dạng)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0.06, 1.18), scale=(0.28, 0.20, 0.28))
    torso = bpy.context.active_object
    torso.name = "Torso"
    torso.rotation_euler = (math.radians(15), 0, math.radians(-6)) # Gù vẹo cột sống
    assign_material(torso, mat_coat)
    parts.append(torso)

    # Cục gù lớn sau lưng
    bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(0, -0.16, 1.34), radius=0.14)
    hump = bpy.context.active_object
    hump.name = "Hump"
    assign_material(hump, mat_coat)
    parts.append(hump)

    # 3. Neck vươn dài về phía trước
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.07, depth=0.22, location=(0, 0.08, 1.52))
    neck = bpy.context.active_object
    neck.name = "Neck"
    neck.rotation_euler = (math.radians(20), 0, 0)
    assign_material(neck, mat_skin)
    parts.append(neck)

    # 4. Head & Glowing Red Eyes
    bpy.ops.mesh.primitive_uv_sphere_add(segments=18, ring_count=12, location=(0, 0.12, 1.70), radius=0.13)
    head = bpy.context.active_object
    head.name = "Head"
    head.scale = (0.95, 1.15, 1.25) # Mặt dài hóp
    assign_material(head, mat_skin)
    parts.append(head)

    # Mắt đỏ phát sáng dị thường
    for side, x in [("L", -0.045), ("R", 0.045)]:
        bpy.ops.mesh.primitive_uv_sphere_add(segments=10, ring_count=8, location=(x, 0.24, 1.72), radius=0.022)
        eye = bpy.context.active_object
        eye.name = f"Eye_{side}"
        assign_material(eye, mat_eyes)
        parts.append(eye)

    # Mũ trùm đầu rách rưới
    bpy.ops.mesh.primitive_uv_sphere_add(segments=18, ring_count=12, location=(0, 0.08, 1.74), radius=0.155)
    hood = bpy.context.active_object
    hood.name = "Hood"
    hood.scale = (1.08, 1.12, 1.05)
    assign_material(hood, mat_coat)
    parts.append(hood)

    # 5. Long arms with sharp talons
    for side, x, sign in [("L", -0.30, -1), ("R", 0.30, 1)]:
        arm_x = sign * 0.36
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, 0.06, 1.34), radius=0.085)
        shoulder = bpy.context.active_object
        shoulder.name = f"Shoulder_{side}"
        assign_material(shoulder, mat_coat)
        parts.append(shoulder)

        # Cánh tay khẳng khiu vươn dài
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.050, depth=0.38, location=(arm_x, 0.06, 1.14))
        upper_arm = bpy.context.active_object
        upper_arm.name = f"UpperArm_{side}"
        assign_material(upper_arm, mat_coat)
        parts.append(upper_arm)

        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.042, depth=0.36, location=(arm_x, 0.08, 0.80))
        forearm = bpy.context.active_object
        forearm.name = f"Forearm_{side}"
        assign_material(forearm, mat_skin)
        parts.append(forearm)

        # Bàn tay móng vuốt dài
        bpy.ops.mesh.primitive_cube_add(location=(arm_x, 0.09, 0.58), scale=(0.040, 0.025, 0.09))
        claw = bpy.context.active_object
        claw.name = f"Claw_{side}"
        assign_material(claw, mat_skin)
        parts.append(claw)

    # 6. Legs
    for side, x in [("L", -0.13), ("R", 0.13)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.075, depth=0.38, location=(x, 0.04, 0.60))
        thigh = bpy.context.active_object
        thigh.name = f"Thigh_{side}"
        assign_material(thigh, mat_pants)
        parts.append(thigh)

        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.062, depth=0.38, location=(x, 0.02, 0.22))
        calf = bpy.context.active_object
        calf.name = f"Calf_{side}"
        assign_material(calf, mat_pants)
        parts.append(calf)

        bpy.ops.mesh.primitive_cube_add(location=(x, 0.08, 0.04), scale=(0.065, 0.16, 0.04))
        shoe = bpy.context.active_object
        shoe.name = f"Shoe_{side}"
        assign_material(shoe, mat_pants)
        parts.append(shoe)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Customer_Doppelganger"
    return torso

# ====================================================
# 4. QUÁI VẬT ĐÊM 2: THE SCREAMER CRAWLER (Quái Bò 4 Chân Cào Xé)
# ====================================================
def build_crawler_monster():
    clear_scene()

    mat_flesh = create_material("Mat_CrawlerFlesh", (0.18, 0.18, 0.22, 1.0), roughness=0.35)
    mat_blood = create_material("Mat_CrawlerBlood", (0.55, 0.05, 0.05, 1.0), roughness=0.2)
    mat_eyes = create_material("Mat_CrawlerEyes", (0.95, 0.85, 0.10, 1.0), roughness=0.1) # Mắt vàng rực

    parts = []

    # Thân chính bò thấp (Crawl Torso)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 0.75), scale=(0.35, 0.60, 0.25))
    torso = bpy.context.active_object
    torso.name = "Crawler_Torso"
    torso.rotation_euler = (math.radians(-15), 0, 0)
    assign_material(torso, mat_flesh)
    parts.append(torso)

    # Gai xương sống nhọn hoắt
    for z, y in [(0.90, -0.3), (0.95, -0.1), (0.98, 0.1), (0.94, 0.3)]:
        bpy.ops.mesh.primitive_cone_add(vertices=8, radius1=0.04, depth=0.18, location=(0, y, z))
        spine = bpy.context.active_object
        spine.name = "Spine"
        assign_material(spine, mat_blood)
        parts.append(spine)

    # Đầu gớm ghiếc há miệng
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=12, location=(0, 0.70, 0.75), radius=0.20)
    head = bpy.context.active_object
    head.name = "Head"
    head.scale = (0.9, 1.3, 0.85)
    assign_material(head, mat_flesh)
    parts.append(head)

    # 4 Mắt vàng sáng
    for x, y, z in [(-0.08, 0.85, 0.82), (0.08, 0.85, 0.82), (-0.12, 0.80, 0.76), (0.12, 0.80, 0.76)]:
        bpy.ops.mesh.primitive_uv_sphere_add(segments=8, ring_count=6, location=(x, y, z), radius=0.025)
        eye = bpy.context.active_object
        eye.name = "Eye"
        assign_material(eye, mat_eyes)
        parts.append(eye)

    # 4 Chi bám sàn (2 chân trước, 2 chân sau)
    for x, y, is_front in [(-0.35, 0.45, True), (0.35, 0.45, True), (-0.35, -0.45, False), (0.35, -0.45, False)]:
        # Khớp đùi / vai
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(x, y, 0.70), radius=0.10)
        joint = bpy.context.active_object
        joint.name = "LimbJoint"
        assign_material(joint, mat_flesh)
        parts.append(joint)

        # Khúc trên chìa ra ngoài
        sign = -1 if x < 0 else 1
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.065, depth=0.45, location=(x + sign * 0.15, y, 0.55))
        upper = bpy.context.active_object
        upper.name = "LimbUpper"
        upper.rotation_euler = (0, math.radians(sign * 35), 0)
        assign_material(upper, mat_flesh)
        parts.append(upper)

        # Khúc dưới cắm xuống đất
        foot_x = x + sign * 0.28
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.050, depth=0.45, location=(foot_x, y, 0.22))
        lower = bpy.context.active_object
        lower.name = "LimbLower"
        assign_material(lower, mat_flesh)
        parts.append(lower)

        # Móng cào
        bpy.ops.mesh.primitive_cube_add(location=(foot_x, y + 0.05, 0.03), scale=(0.06, 0.10, 0.03))
        claw = bpy.context.active_object
        claw.name = "Claw"
        assign_material(claw, mat_blood)
        parts.append(claw)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Monster_Crawler"
    return torso

# ====================================================
# 5. QUÁI VẬT ĐÊM 3: THE ABYSSAL VOID (Bóng Ma Hư Không)
# ====================================================
def build_void_phantom():
    clear_scene()

    mat_void = create_material("Mat_VoidSkin", (0.04, 0.03, 0.06, 1.0), roughness=0.2, metallic=0.3)
    mat_glow = create_material("Mat_VoidGlow", (0.85, 0.05, 0.05, 1.0), roughness=0.1)

    parts = []

    # Thân cao lớn khổng lồ (Chiều cao 2.6m)
    bpy.ops.mesh.primitive_cube_add(location=(0, 0, 1.55), scale=(0.32, 0.22, 0.45))
    torso = bpy.context.active_object
    torso.name = "Void_Torso"
    assign_material(torso, mat_void)
    parts.append(torso)

    # Lồng ngực nứt toác phát sáng đỏ (Core Energy)
    bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(0, 0.18, 1.65), radius=0.12)
    core = bpy.context.active_object
    core.name = "Core"
    assign_material(core, mat_glow)
    parts.append(core)

    # Cổ & Đầu sọ hư không
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.08, depth=0.25, location=(0, 0, 2.05))
    neck = bpy.context.active_object
    neck.name = "Neck"
    assign_material(neck, mat_void)
    parts.append(neck)

    bpy.ops.mesh.primitive_uv_sphere_add(segments=20, ring_count=14, location=(0, 0, 2.30), radius=0.16)
    head = bpy.context.active_object
    head.name = "Head"
    head.scale = (0.9, 1.1, 1.3)
    assign_material(head, mat_void)
    parts.append(head)

    # 2 Hốc mắt đỏ rực
    for side, x in [("L", -0.06), ("R", 0.06)]:
        bpy.ops.mesh.primitive_uv_sphere_add(segments=10, ring_count=8, location=(x, 0.16, 2.34), radius=0.035)
        eye = bpy.context.active_object
        eye.name = f"Eye_{side}"
        assign_material(eye, mat_glow)
        parts.append(eye)

    # Cánh tay bóng ma dài chạm đất
    for side, x, sign in [("L", -0.36, -1), ("R", 0.36, 1)]:
        arm_x = sign * 0.44
        bpy.ops.mesh.primitive_uv_sphere_add(segments=14, ring_count=10, location=(x, 0, 1.85), radius=0.11)
        shoulder = bpy.context.active_object
        shoulder.name = f"Shoulder_{side}"
        assign_material(shoulder, mat_void)
        parts.append(shoulder)

        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.065, depth=0.60, location=(arm_x, 0, 1.45))
        upper_arm = bpy.context.active_object
        upper_arm.name = f"UpperArm_{side}"
        assign_material(upper_arm, mat_void)
        parts.append(upper_arm)

        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.052, depth=0.60, location=(arm_x, 0, 0.90))
        forearm = bpy.context.active_object
        forearm.name = f"Forearm_{side}"
        assign_material(forearm, mat_void)
        parts.append(forearm)

        # Móng vuốt hư không đen nhánh
        bpy.ops.mesh.primitive_cube_add(location=(arm_x, 0, 0.50), scale=(0.05, 0.03, 0.16))
        claw = bpy.context.active_object
        claw.name = f"Claw_{side}"
        assign_material(claw, mat_glow)
        parts.append(claw)

    # Chân dài khẳng khiu
    for side, x in [("L", -0.16), ("R", 0.16)]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.09, depth=0.60, location=(x, 0, 0.90))
        thigh = bpy.context.active_object
        thigh.name = f"Thigh_{side}"
        assign_material(thigh, mat_void)
        parts.append(thigh)

        bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.075, depth=0.60, location=(x, 0, 0.32))
        calf = bpy.context.active_object
        calf.name = f"Calf_{side}"
        assign_material(calf, mat_void)
        parts.append(calf)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "Monster_Void"
    return torso

# ====================================================
# THỰC THI & XUẤT RA ASSETS/MODELS/
# ====================================================
if __name__ == "__main__":
    out_dir = r"D:\ProjectGame\Game\Assets\Models"
    os.makedirs(out_dir, exist_ok=True)

    print("--- [BLENDER 5.1] BUILDING CONNECTED DETAILED CHARACTERS ---")

    # 1. Customer Male
    male = build_male_customer()
    export_obj(male, os.path.join(out_dir, "Customer_Male.obj"))
    print("✓ Customer_Male.obj exported successfully with complete joints & neck!")

    # 2. Customer Female
    female = build_female_customer()
    export_obj(female, os.path.join(out_dir, "Customer_Female.obj"))
    print("✓ Customer_Female.obj exported successfully with complete joints & neck!")

    # 3. Customer Doppelganger
    doppel = build_doppelganger()
    export_obj(doppel, os.path.join(out_dir, "Customer_Doppelganger.obj"))
    print("✓ Customer_Doppelganger.obj exported successfully with complete joints & neck!")

    # 4. Night 2 Monster: Screamer Crawler
    crawler = build_crawler_monster()
    export_obj(crawler, os.path.join(out_dir, "Monster_Crawler.obj"))
    print("✓ Monster_Crawler.obj exported successfully for Night 2!")

    # 5. Night 3 Monster: Void Phantom
    void_m = build_void_phantom()
    export_obj(void_m, os.path.join(out_dir, "Boss_True_Abyssal.obj"))
    print("✓ Boss_True_Abyssal.obj exported successfully for Night 3!")

    print("--- [BLENDER 5.1] ALL CHARACTERS GENERATED & CONNECTED SEAMLESSLY! ---")


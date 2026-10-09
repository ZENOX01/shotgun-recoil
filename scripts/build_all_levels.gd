extends SceneTree

func _init():
	print("--- Generating Levels 2, 3, 4, and 5 with new JumpPad assets and clean high-contrast visuals ---")

	var base_scene = load("res://scenes/level_1.tscn")
	if not base_scene:
		printerr("Could not load level_1.tscn")
		quit(1)
		return

	var sample = base_scene.instantiate()
	var sample_plat = sample.get_node("tilemaps/platforms") as TileMapLayer
	var sample_bg = sample.get_node("tilemaps/Bg") as TileMapLayer
	var sample_dec = sample.get_node("tilemaps/decorations") as TileMapLayer

	var plat_ts = sample_plat.tile_set
	var bg_ts = sample_bg.tile_set
	var dec_ts = sample_dec.tile_set

	var player_scene = load("res://scenes/player.tscn")
	var goal_scene = load("res://scenes/Goal.tscn")
	var spike_scene = load("res://scenes/spikes.tscn")
	var hud_scene = load("res://scenes/AmmoHUD.tscn")
	var main_script = load("res://scripts/Main.cs")

	var breakable_scene = load("res://scenes/BreakableWall.tscn")
	var moving_spikes_scene = load("res://scenes/MovingSpikes.tscn")
	var ammo_refill_scene = load("res://scenes/AmmoRefill.tscn")
	var jump_pad_scene = load("res://scenes/JumpPad.tscn")

	# Helper to create clean level base
	var create_level_base = func(level_name: String):
		var root = Node2D.new()
		root.name = level_name
		root.set_script(main_script)

		var tilemaps = Node2D.new()
		tilemaps.name = "tilemaps"
		root.add_child(tilemaps)
		tilemaps.owner = root

		# Dark recessed background cave wall
		var bg_layer = TileMapLayer.new()
		bg_layer.name = "Bg"
		bg_layer.tile_set = bg_ts
		bg_layer.modulate = Color(0.22, 0.15, 0.32, 0.6)
		tilemaps.add_child(bg_layer)
		bg_layer.owner = root

		# Crisp, high-contrast foreground platforms
		var plat_layer = TileMapLayer.new()
		plat_layer.name = "platforms"
		plat_layer.tile_set = plat_ts
		plat_layer.modulate = Color(1.0, 1.0, 1.0, 1.0)
		tilemaps.add_child(plat_layer)
		plat_layer.owner = root

		# Clean decorations (vines, bushes)
		var dec_layer = TileMapLayer.new()
		dec_layer.name = "decorations"
		dec_layer.tile_set = dec_ts
		tilemaps.add_child(dec_layer)
		dec_layer.owner = root

		var hazards_node = Node2D.new()
		hazards_node.name = "Hazards"
		root.add_child(hazards_node)
		hazards_node.owner = root

		var bullets_node = Node2D.new()
		bullets_node.name = "Bullets"
		root.add_child(bullets_node)
		bullets_node.owner = root
		root.set("Bullets", bullets_node)

		var obstacles_node = Node2D.new()
		obstacles_node.name = "Obstacles"
		root.add_child(obstacles_node)
		obstacles_node.owner = root

		return {
			"root": root,
			"bg": bg_layer,
			"plat": plat_layer,
			"dec": dec_layer,
			"hazards": hazards_node,
			"obstacles": obstacles_node
		}

	var place_block = func(layer: TileMapLayer, x: int, y: int, w: int, h: int):
		for r in range(h):
			for c in range(w):
				var tile_x: int
				var tile_y: int
				if h == 1:
					tile_y = 4
				elif r == 0:
					tile_y = 4
				elif r == h - 1:
					tile_y = 6
				else:
					tile_y = 5

				if w == 1:
					tile_x = 7
				elif c == 0:
					tile_x = 5
				elif c == w - 1:
					tile_x = 9
				else:
					tile_x = 6 + (c % 3)
				layer.set_cell(Vector2i(x + c, y + r), 0, Vector2i(tile_x, tile_y))

	var place_stone = func(layer: TileMapLayer, x: int, y: int, w: int, h: int):
		for r in range(h):
			for c in range(w):
				var tile_x = 11
				var tile_y = 8
				if c == 0 and w > 1: tile_x = 10
				elif c == w - 1 and w > 1: tile_x = 12
				else: tile_x = 11

				if r == 0 and h > 1: tile_y = 8
				elif r == h - 1 and h > 1: tile_y = 10
				elif h == 1: tile_y = 10
				else: tile_y = 9
				layer.set_cell(Vector2i(x + c, y + r), 0, Vector2i(tile_x, tile_y))

	var fill_bg = func(bg: TileMapLayer, x: int, y: int, w: int, h: int):
		for r in range(h):
			for c in range(w):
				bg.set_cell(Vector2i(x + c, y + r), 0, Vector2i(6, 5))

	var add_spike_line = func(root_node: Node2D, hazards_node: Node2D, start_x: int, end_x: int, tile_y: int):
		for sx in range(start_x, end_x + 1):
			var spike = spike_scene.instantiate()
			spike.position = Vector2(sx * 16 + 8, tile_y * 16 - 8)
			hazards_node.add_child(spike)
			spike.owner = root_node

	var add_hanging_vine = func(dec: TileMapLayer, x: int, start_y: int, length: int):
		for r in range(length):
			if r == 0:
				dec.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 11))
			elif r == length - 1:
				dec.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 13))
			else:
				dec.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 12))

	var add_bush = func(dec: TileMapLayer, x: int, y: int, variant: int = 0):
		dec.set_cell(Vector2i(x, y), 0, Vector2i(2 + (variant % 4), 11))

	var finalize_level = func(root_node: Node2D, player_pos: Vector2, goal_pos: Vector2, next_level_path: String, level_title: String, save_path: String):
		var player = player_scene.instantiate()
		player.position = player_pos
		root_node.add_child(player)
		player.owner = root_node
		player.connect("ShootBullet", Callable(root_node, "_on_player_shoot_bullet"), CONNECT_PERSIST)

		var goal = goal_scene.instantiate()
		goal.position = goal_pos
		goal.set("NextLevelPath", next_level_path)
		goal.set("LevelTitle", level_title)
		root_node.add_child(goal)
		goal.owner = root_node

		var hud = hud_scene.instantiate()
		root_node.add_child(hud)
		hud.owner = root_node

		var packed = PackedScene.new()
		var res = packed.pack(root_node)
		if res != OK:
			printerr("Failed to pack scene: ", save_path)
			return false
		var save_res = ResourceSaver.save(packed, save_path)
		if save_res != OK:
			printerr("Failed to save scene: ", save_path)
			return false
		print("Successfully saved: ", save_path)
		root_node.queue_free()
		return true

	# =========================================================================
	# LEVEL 2: "The Timber Chasm" (Difficulty ★★☆☆☆)
	# =========================================================================
	print("\n--- Generating Level 2: The Timber Chasm ---")
	var l2 = create_level_base.call("Level2")
	var r2 = l2.root
	var p2 = l2.plat
	var b2 = l2.bg
	var d2 = l2.dec
	var h2 = l2.hazards
	var o2 = l2.obstacles

	# Boundaries
	place_block.call(p2, 0, 0, 2, 14)
	place_block.call(p2, 86, 0, 2, 14)
	place_block.call(p2, 0, -2, 88, 2)

	# Section 1: Spawn Ledge
	place_block.call(p2, 2, 9, 6, 5)
	add_bush.call(d2, 5, 8, 0)
	add_bush.call(d2, 7, 8, 1)

	# Section 2: Drop Pit with NEW Spring JumpPad!
	place_block.call(p2, 8, 13, 8, 3)
	fill_bg.call(b2, 8, 9, 8, 4)

	# NEW Jump Pad at x=12, y=13
	var pad2 = jump_pad_scene.instantiate()
	pad2.position = Vector2(12 * 16 + 8, 13 * 16)
	pad2.set("LaunchForce", 420.0)
	o2.add_child(pad2)
	pad2.owner = r2

	# High Overlook Platform after JumpPad launch
	place_block.call(p2, 18, 6, 6, 8)
	add_bush.call(d2, 21, 5, 2)

	# Section 3: Breakable Stone Gate blocking narrow pass
	var wall2 = breakable_scene.instantiate()
	wall2.position = Vector2(25 * 16 + 8, 5 * 16 + 8)
	o2.add_child(wall2)
	wall2.owner = r2

	place_block.call(p2, 24, 7, 6, 7)
	place_stone.call(p2, 24, 3, 4, 1)

	# Section 4: The Great Timber Chasm (Spikes floor at y=15)
	place_block.call(p2, 30, 15, 24, 3)
	add_spike_line.call(r2, h2, 30, 53, 15)
	fill_bg.call(b2, 30, 7, 24, 8)

	# Floating Ammo Refill Crystals across the chasm
	var refill2_1 = ammo_refill_scene.instantiate()
	refill2_1.position = Vector2(37 * 16 + 8, 7 * 16)
	o2.add_child(refill2_1)
	refill2_1.owner = r2

	# Mid stepping stone in chasm
	place_stone.call(p2, 41, 8, 3, 1)
	add_bush.call(d2, 42, 7, 3)

	var refill2_2 = ammo_refill_scene.instantiate()
	refill2_2.position = Vector2(48 * 16 + 8, 7 * 16)
	o2.add_child(refill2_2)
	refill2_2.owner = r2

	# Section 5: Landing & Final Ascent
	place_block.call(p2, 54, 8, 7, 6)
	add_bush.call(d2, 56, 7, 0)

	place_block.call(p2, 63, 6, 5, 8)
	place_stone.call(p2, 69, 4, 3, 1)
	place_block.call(p2, 74, 8, 12, 6)
	add_bush.call(d2, 78, 7, 1)

	# Ceiling hanging vines
	add_hanging_vine.call(d2, 14, 0, 4)
	add_hanging_vine.call(d2, 35, 0, 4)
	add_hanging_vine.call(d2, 50, 0, 3)
	add_hanging_vine.call(d2, 68, 0, 4)

	finalize_level.call(r2, Vector2(4 * 16 + 8, 8 * 16), Vector2(82 * 16 + 8, 8 * 16), "res://scenes/level_3.tscn", "★ LEVEL 2 COMPLETE! ★", "res://scenes/level_2.tscn")

	# =========================================================================
	# LEVEL 3: "The Crumbling Bastion" (Difficulty ★★★☆☆)
	# =========================================================================
	print("\n--- Generating Level 3: The Crumbling Bastion ---")
	var l3 = create_level_base.call("Level3")
	var r3 = l3.root
	var p3 = l3.plat
	var b3 = l3.bg
	var d3 = l3.dec
	var h3 = l3.hazards
	var o3 = l3.obstacles

	# Boundaries
	place_block.call(p3, 0, -4, 2, 22)
	place_block.call(p3, 88, -4, 2, 22)
	place_block.call(p3, 0, -6, 90, 2)

	# Section 1: Fortress Entrance
	place_block.call(p3, 2, 10, 8, 5)
	add_bush.call(d3, 6, 9, 2)

	# Section 2: Moving Spikes Corridor
	place_block.call(p3, 10, 15, 14, 3)
	add_spike_line.call(r3, h3, 10, 23, 15)
	fill_bg.call(b3, 10, 8, 14, 7)

	place_stone.call(p3, 13, 9, 3, 1)
	place_stone.call(p3, 19, 9, 3, 1)

	var mov_spk1 = moving_spikes_scene.instantiate()
	mov_spk1.position = Vector2(16 * 16 + 8, 8 * 16)
	mov_spk1.set("MoveOffset", Vector2(40, 0))
	mov_spk1.set("Speed", 55.0)
	o3.add_child(mov_spk1)
	mov_spk1.owner = r3

	# Section 3: Mid Platform
	place_block.call(p3, 24, 9, 6, 6)
	add_bush.call(d3, 26, 8, 0)

	# Section 4: Vertical Shaft with JumpPad launch!
	place_block.call(p3, 30, -2, 2, 17)
	place_block.call(p3, 37, -2, 2, 17)
	fill_bg.call(b3, 32, -2, 5, 17)

	place_block.call(p3, 32, 15, 5, 2)
	add_spike_line.call(r3, h3, 32, 36, 15)

	# NEW JumpPad at base of shaft at x=34, y=14
	var pad3 = jump_pad_scene.instantiate()
	pad3.position = Vector2(34 * 16 + 8, 14 * 16)
	pad3.set("LaunchForce", 450.0)
	o3.add_child(pad3)
	pad3.owner = r3

	var refill3_1 = ammo_refill_scene.instantiate()
	refill3_1.position = Vector2(34 * 16 + 8, 6 * 16)
	o3.add_child(refill3_1)
	refill3_1.owner = r3

	var wall3_1 = breakable_scene.instantiate()
	wall3_1.position = Vector2(37 * 16 + 8, 1 * 16 + 8)
	o3.add_child(wall3_1)
	wall3_1.owner = r3

	# Section 5: Rooftop Battlement (y=3)
	place_block.call(p3, 39, 3, 10, 12)
	add_bush.call(d3, 42, 2, 1)

	var mov_spk2 = moving_spikes_scene.instantiate()
	mov_spk2.position = Vector2(43 * 16 + 8, 2 * 16)
	mov_spk2.set("MoveOffset", Vector2(48, 0))
	mov_spk2.set("Speed", 65.0)
	o3.add_child(mov_spk2)
	mov_spk2.owner = r3

	# Section 6: High Drop & Second Blast Gate
	place_block.call(p3, 52, 14, 18, 3)
	add_spike_line.call(r3, h3, 52, 69, 14)
	fill_bg.call(b3, 52, 3, 18, 11)

	place_stone.call(p3, 55, 3, 3, 1)
	place_stone.call(p3, 62, 5, 3, 1)

	var wall3_2 = breakable_scene.instantiate()
	wall3_2.position = Vector2(67 * 16 + 8, 5 * 16 + 8)
	o3.add_child(wall3_2)
	wall3_2.owner = r3

	var refill3_2 = ammo_refill_scene.instantiate()
	refill3_2.position = Vector2(59 * 16 + 8, 4 * 16)
	o3.add_child(refill3_2)
	refill3_2.owner = r3

	# Section 7: Goal Sanctuary
	place_block.call(p3, 70, 7, 18, 8)
	add_bush.call(d3, 74, 6, 2)
	add_bush.call(d3, 80, 6, 3)

	finalize_level.call(r3, Vector2(4 * 16 + 8, 9 * 16), Vector2(82 * 16 + 8, 7 * 16), "res://scenes/level_4.tscn", "★ LEVEL 3 COMPLETE! ★", "res://scenes/level_3.tscn")

	# =========================================================================
	# LEVEL 4: "The Spike Gauntlet" (Difficulty ★★★★☆)
	# =========================================================================
	print("\n--- Generating Level 4: The Spike Gauntlet ---")
	var l4 = create_level_base.call("Level4")
	var r4 = l4.root
	var p4 = l4.plat
	var b4 = l4.bg
	var d4 = l4.dec
	var h4 = l4.hazards
	var o4 = l4.obstacles

	# Boundaries
	place_block.call(p4, 0, -6, 2, 24)
	place_block.call(p4, 96, -6, 2, 24)
	place_block.call(p4, 0, -8, 98, 2)

	# Section 1: Spawn
	place_block.call(p4, 2, 10, 6, 5)
	add_bush.call(d4, 4, 9, 0)

	# Section 2: Low-Ceiling Spike Tunnel
	place_block.call(p4, 8, 10, 10, 5)
	place_stone.call(p4, 8, 5, 10, 2)
	fill_bg.call(b4, 8, 7, 10, 3)

	var mov_spk4_1 = moving_spikes_scene.instantiate()
	mov_spk4_1.position = Vector2(12 * 16 + 8, 9 * 16)
	mov_spk4_1.set("MoveOffset", Vector2(48, 0))
	mov_spk4_1.set("Speed", 70.0)
	o4.add_child(mov_spk4_1)
	mov_spk4_1.owner = r4

	# Section 3: The Sunken Crypt Spike Lake
	place_block.call(p4, 18, 15, 32, 3)
	add_spike_line.call(r4, h4, 18, 49, 15)
	fill_bg.call(b4, 18, 4, 32, 11)

	place_stone.call(p4, 23, 9, 2, 1)
	place_stone.call(p4, 31, 7, 2, 1)
	place_stone.call(p4, 39, 8, 2, 1)
	place_stone.call(p4, 46, 6, 2, 1)

	var refill4_1 = ammo_refill_scene.instantiate()
	refill4_1.position = Vector2(27 * 16 + 8, 7 * 16)
	o4.add_child(refill4_1)
	refill4_1.owner = r4

	var refill4_2 = ammo_refill_scene.instantiate()
	refill4_2.position = Vector2(35 * 16 + 8, 6 * 16)
	o4.add_child(refill4_2)
	refill4_2.owner = r4

	var refill4_3 = ammo_refill_scene.instantiate()
	refill4_3.position = Vector2(43 * 16 + 8, 5 * 16)
	o4.add_child(refill4_3)
	refill4_3.owner = r4

	# Vertical crusher spikes
	var mov_crush1 = moving_spikes_scene.instantiate()
	mov_crush1.position = Vector2(31 * 16 + 8, 3 * 16)
	mov_crush1.set("MoveOffset", Vector2(0, 36))
	mov_crush1.set("Speed", 45.0)
	o4.add_child(mov_crush1)
	mov_crush1.owner = r4

	# Section 4: Mid Island with Breakable Gate
	place_block.call(p4, 50, 7, 8, 8)
	add_bush.call(d4, 52, 6, 1)

	var wall4_1 = breakable_scene.instantiate()
	wall4_1.position = Vector2(57 * 16 + 8, 5 * 16 + 8)
	o4.add_child(wall4_1)
	wall4_1.owner = r4

	# Section 5: Upper Gauntlet with JumpPad launch!
	place_block.call(p4, 58, 14, 20, 3)
	add_spike_line.call(r4, h4, 58, 77, 14)
	fill_bg.call(b4, 58, 2, 20, 12)

	# NEW JumpPad at x=61, y=13
	var pad4 = jump_pad_scene.instantiate()
	pad4.position = Vector2(61 * 16 + 8, 13 * 16)
	pad4.set("LaunchForce", 440.0)
	o4.add_child(pad4)
	pad4.owner = r4

	place_stone.call(p4, 66, 3, 3, 1)
	place_stone.call(p4, 73, 4, 3, 1)

	var mov_spk4_2 = moving_spikes_scene.instantiate()
	mov_spk4_2.position = Vector2(69 * 16 + 8, 2 * 16)
	mov_spk4_2.set("MoveOffset", Vector2(40, 0))
	mov_spk4_2.set("Speed", 80.0)
	o4.add_child(mov_spk4_2)
	mov_spk4_2.owner = r4

	var refill4_4 = ammo_refill_scene.instantiate()
	refill4_4.position = Vector2(70 * 16 + 8, 2 * 16)
	o4.add_child(refill4_4)
	refill4_4.owner = r4

	# Section 6: Crypt Sanctuary Exit
	place_block.call(p4, 78, 5, 18, 10)
	add_bush.call(d4, 80, 4, 2)
	add_bush.call(d4, 86, 4, 3)

	finalize_level.call(r4, Vector2(4 * 16 + 8, 9 * 16), Vector2(90 * 16 + 8, 5 * 16), "res://scenes/level_5.tscn", "★ LEVEL 4 COMPLETE! ★", "res://scenes/level_4.tscn")

	# =========================================================================
	# LEVEL 5: "Ascent of the Gunner" (Grand Finale - Difficulty ★★★★★)
	# =========================================================================
	print("\n--- Generating Level 5: Ascent of the Gunner ---")
	var l5 = create_level_base.call("Level5")
	var r5 = l5.root
	var p5 = l5.plat
	var b5 = l5.bg
	var d5 = l5.dec
	var h5 = l5.hazards
	var o5 = l5.obstacles

	# Boundaries
	place_block.call(p5, 0, -10, 2, 28)
	place_block.call(p5, 104, -10, 2, 28)
	place_block.call(p5, 0, -12, 106, 2)

	# Section 1: Summit Base
	place_block.call(p5, 2, 11, 7, 5)
	add_bush.call(d5, 5, 10, 0)

	# Part 1: JumpPad launch across first chasm
	place_block.call(p5, 9, 15, 16, 3)
	add_spike_line.call(r5, h5, 9, 24, 15)
	fill_bg.call(b5, 9, 4, 16, 11)

	# NEW JumpPad at x=11, y=14
	var pad5_1 = jump_pad_scene.instantiate()
	pad5_1.position = Vector2(11 * 16 + 8, 14 * 16)
	pad5_1.set("LaunchForce", 450.0)
	o5.add_child(pad5_1)
	pad5_1.owner = r5

	var mov_spk5_1 = moving_spikes_scene.instantiate()
	mov_spk5_1.position = Vector2(17 * 16 + 8, 4 * 16)
	mov_spk5_1.set("MoveOffset", Vector2(45, 0))
	mov_spk5_1.set("Speed", 85.0)
	o5.add_child(mov_spk5_1)
	mov_spk5_1.owner = r5

	var refill5_1 = ammo_refill_scene.instantiate()
	refill5_1.position = Vector2(19 * 16 + 8, 3 * 16)
	o5.add_child(refill5_1)
	refill5_1.owner = r5

	# Part 2: High Stepping Stone & Mid-air Breakable Gate
	place_stone.call(p5, 25, 4, 4, 1)
	add_bush.call(d5, 26, 3, 1)

	var wall5_1 = breakable_scene.instantiate()
	wall5_1.position = Vector2(31 * 16 + 8, 2 * 16 + 8)
	o5.add_child(wall5_1)
	wall5_1.owner = r5

	place_block.call(p5, 25, 15, 18, 3)
	add_spike_line.call(r5, h5, 25, 42, 15)
	fill_bg.call(b5, 25, 2, 18, 13)

	place_stone.call(p5, 34, 3, 3, 1)

	# Part 3: The Sky Spire Ascent
	place_block.call(p5, 40, 2, 6, 14)

	var mov_crush5 = moving_spikes_scene.instantiate()
	mov_crush5.position = Vector2(38 * 16 + 8, -2 * 16)
	mov_crush5.set("MoveOffset", Vector2(0, 50))
	mov_crush5.set("Speed", 60.0)
	o5.add_child(mov_crush5)
	mov_crush5.owner = r5

	# Spire summit JumpPad at x=43, y=1
	var pad5_2 = jump_pad_scene.instantiate()
	pad5_2.position = Vector2(43 * 16 + 8, 1 * 16)
	pad5_2.set("LaunchForce", 460.0)
	o5.add_child(pad5_2)
	pad5_2.owner = r5

	# Part 4: The Great Void Chasm
	place_block.call(p5, 46, 15, 32, 3)
	add_spike_line.call(r5, h5, 46, 77, 15)
	fill_bg.call(b5, 46, -4, 32, 19)

	var refill5_2 = ammo_refill_scene.instantiate()
	refill5_2.position = Vector2(51 * 16 + 8, -3 * 16)
	o5.add_child(refill5_2)
	refill5_2.owner = r5

	place_stone.call(p5, 56, -2, 2, 1)

	var mov_spk5_2 = moving_spikes_scene.instantiate()
	mov_spk5_2.position = Vector2(60 * 16 + 8, -4 * 16)
	mov_spk5_2.set("MoveOffset", Vector2(40, 0))
	mov_spk5_2.set("Speed", 90.0)
	o5.add_child(mov_spk5_2)
	mov_spk5_2.owner = r5

	var refill5_3 = ammo_refill_scene.instantiate()
	refill5_3.position = Vector2(62 * 16 + 8, -5 * 16)
	o5.add_child(refill5_3)
	refill5_3.owner = r5

	place_stone.call(p5, 68, -3, 2, 1)

	var refill5_4 = ammo_refill_scene.instantiate()
	refill5_4.position = Vector2(74 * 16 + 8, -4 * 16)
	o5.add_child(refill5_4)
	refill5_4.owner = r5

	# Final Blast Gate guarding temple entrance
	var wall5_2 = breakable_scene.instantiate()
	wall5_2.position = Vector2(78 * 16 + 8, -5 * 16 + 8)
	o5.add_child(wall5_2)
	wall5_2.owner = r5

	# Part 5: The Grand Golden Summit Sanctuary
	place_block.call(p5, 78, -3, 26, 18)
	add_bush.call(d5, 82, -4, 0)
	add_bush.call(d5, 88, -4, 1)
	add_bush.call(d5, 94, -4, 2)

	# Ceiling hanging vines
	add_hanging_vine.call(d5, 15, -10, 4)
	add_hanging_vine.call(d5, 35, -10, 3)
	add_hanging_vine.call(d5, 55, -10, 4)
	add_hanging_vine.call(d5, 75, -10, 3)

	finalize_level.call(r5, Vector2(4 * 16 + 8, 10 * 16), Vector2(92 * 16 + 8, -4 * 16), "", "★ ALL LEVELS CLEARED! ★", "res://scenes/level_5.tscn")

	print("\nALL LEVELS GENERATED WITH NEW JUMP PADS AND CLEAN VISUALS!")
	sample.queue_free()
	quit(0)

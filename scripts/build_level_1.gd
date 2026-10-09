extends SceneTree

func _init():
	print("Building Level 1 with clean, high-contrast platforms and dark recessed background...")
	var existing_scene = load("res://scenes/level_1.tscn")
	if not existing_scene:
		printerr("Could not load existing level_1.tscn")
		quit(1)
		return

	var orig_root = existing_scene.instantiate()
	var orig_platforms = orig_root.get_node("tilemaps/platforms") as TileMapLayer
	var orig_bg = orig_root.get_node("tilemaps/Bg") as TileMapLayer
	var orig_dec = orig_root.get_node("tilemaps/decorations") as TileMapLayer

	var platforms_ts = orig_platforms.tile_set
	var bg_ts = orig_bg.tile_set
	var dec_ts = orig_dec.tile_set

	var spike_scene = load("res://scenes/spikes.tscn")
	var player_scene = load("res://scenes/player.tscn")
	var goal_scene = load("res://scenes/Goal.tscn")
	var hud_scene = load("res://scenes/AmmoHUD.tscn")
	var main_script = load("res://scripts/Main.cs")

	# Root Node
	var root = Node2D.new()
	root.name = "Level1"
	root.set_script(main_script)

	# --- 1. TILEMAPS ---
	var tilemaps = Node2D.new()
	tilemaps.name = "tilemaps"
	root.add_child(tilemaps)
	tilemaps.owner = root

	# Dark, desaturated, shadowy cavern background layer
	# Pushed way back in visual depth so solid platforms POP completely!
	var bg_layer = TileMapLayer.new()
	bg_layer.name = "Bg"
	bg_layer.tile_set = bg_ts
	bg_layer.modulate = Color(0.22, 0.15, 0.32, 0.6)
	tilemaps.add_child(bg_layer)
	bg_layer.owner = root

	# Platform / Collision tile layer (full brightness 100% white, high-contrast, crystal clear)
	var plat_layer = TileMapLayer.new()
	plat_layer.name = "platforms"
	plat_layer.tile_set = platforms_ts
	plat_layer.modulate = Color(1.0, 1.0, 1.0, 1.0)
	tilemaps.add_child(plat_layer)
	plat_layer.owner = root

	# Decorations tile layer (vines, bushes, ruin markers)
	var dec_layer = TileMapLayer.new()
	dec_layer.name = "decorations"
	dec_layer.tile_set = dec_ts
	tilemaps.add_child(dec_layer)
	dec_layer.owner = root

	# Spikes container
	var spikes_node = Node2D.new()
	spikes_node.name = "Spikes"
	root.add_child(spikes_node)
	spikes_node.owner = root

	# Bullets container
	var bullets_node = Node2D.new()
	bullets_node.name = "Bullets"
	root.add_child(bullets_node)
	bullets_node.owner = root
	root.set("Bullets", bullets_node)

	# --- HELPERS ---
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

	var add_spike = func(tile_x: int, tile_y: int):
		var spike = spike_scene.instantiate()
		spike.position = Vector2(tile_x * 16 + 8, tile_y * 16 - 8)
		spikes_node.add_child(spike)
		spike.owner = root

	var add_spike_line = func(start_x: int, end_x: int, tile_y: int):
		for sx in range(start_x, end_x + 1):
			add_spike.call(sx, tile_y)

	var fill_bg = func(x: int, y: int, w: int, h: int):
		for r in range(h):
			for c in range(w):
				bg_layer.set_cell(Vector2i(x + c, y + r), 0, Vector2i(6, 5))

	var add_hanging_vine = func(x: int, start_y: int, length: int):
		for r in range(length):
			if r == 0:
				dec_layer.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 11))
			elif r == length - 1:
				dec_layer.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 13))
			else:
				dec_layer.set_cell(Vector2i(x, start_y + r), 0, Vector2i(2 + (x % 4), 12))

	var add_bush = func(x: int, y: int, variant: int = 0):
		dec_layer.set_cell(Vector2i(x, y), 0, Vector2i(2 + (variant % 4), 11))

	# --- 2. FOREGROUND PLATFORMS & GEOMETRY ---
	# Left border wall
	place_block.call(plat_layer, 0, 0, 2, 14)

	# Spawn platform
	place_block.call(plat_layer, 2, 9, 6, 5)

	# Tutorial wall at x=8..9, y=5..13
	place_block.call(plat_layer, 8, 5, 2, 9)

	# Landing platform after barrier
	place_block.call(plat_layer, 10, 9, 5, 5)

	# Pit 1 floor (under spikes)
	place_block.call(plat_layer, 15, 14, 6, 3)
	add_spike_line.call(15, 20, 14)

	# Mid-Island 1
	place_block.call(plat_layer, 21, 9, 5, 5)

	# Pit 2 floor (under spikes)
	place_block.call(plat_layer, 26, 14, 8, 3)
	add_spike_line.call(26, 33, 14)

	# Mid floating stone stepping stone in Pit 2
	place_stone.call(plat_layer, 29, 6, 3, 1)

	# Landing platform 2
	place_block.call(plat_layer, 34, 9, 6, 5)

	# The Ascent - Stepped mountain
	place_block.call(plat_layer, 41, 7, 4, 7)
	place_block.call(plat_layer, 45, 4, 4, 10)
	place_block.call(plat_layer, 49, 1, 5, 13)

	# Pit 3 (The Great Gorge)
	place_block.call(plat_layer, 54, 15, 15, 3)
	add_spike_line.call(54, 68, 15)

	# Floating stepping stones across the gorge
	place_stone.call(plat_layer, 57, 5, 3, 1)
	place_stone.call(plat_layer, 63, 8, 3, 1)

	# Final Goal Platform
	place_block.call(plat_layer, 69, 9, 9, 5)

	# Right boundary wall
	place_block.call(plat_layer, 78, 0, 2, 14)

	# Ceiling boundary
	place_block.call(plat_layer, 0, -2, 80, 2)

	# --- 3. BACKGROUND TILES (Dark shadowy cavern wall) ---
	# Deep background cavern backing only behind pits and trenches
	# (Recessed at 60% opacity, deep purple, completely behind solid platforms!)
	fill_bg.call(15, 8, 6, 6)
	fill_bg.call(26, 8, 8, 6)
	fill_bg.call(54, 7, 15, 8)

	# --- 4. FOREGROUND DECORATIONS ---
	# Surface bushes on platforms
	add_bush.call(4, 8, 0)
	add_bush.call(11, 8, 1)
	add_bush.call(22, 8, 2)
	add_bush.call(30, 5, 3)
	add_bush.call(35, 8, 0)
	add_bush.call(42, 6, 1)
	add_bush.call(46, 3, 2)
	add_bush.call(50, 0, 3)
	add_bush.call(58, 4, 0)
	add_bush.call(64, 7, 1)
	add_bush.call(71, 8, 2)
	add_bush.call(75, 8, 3)

	# Hanging vines dropping from ceiling
	add_hanging_vine.call(5, 0, 3)
	add_hanging_vine.call(13, 0, 4)
	add_hanging_vine.call(23, 0, 3)
	add_hanging_vine.call(33, 0, 4)
	add_hanging_vine.call(43, 0, 3)
	add_hanging_vine.call(55, 0, 4)
	add_hanging_vine.call(65, 0, 3)
	add_hanging_vine.call(73, 0, 4)

	# --- 5. PLAYER ---
	var player = player_scene.instantiate()
	player.position = Vector2(4 * 16 + 8, 8 * 16)
	root.add_child(player)
	player.owner = root
	player.connect("ShootBullet", Callable(root, "_on_player_shoot_bullet"), CONNECT_PERSIST)

	# --- 6. GOAL ---
	var goal = goal_scene.instantiate()
	goal.position = Vector2(74 * 16 + 8, 9 * 16)
	goal.set("NextLevelPath", "res://scenes/level_2.tscn")
	goal.set("LevelTitle", "★ LEVEL 1 COMPLETE! ★")
	root.add_child(goal)
	goal.owner = root

	# --- 7. HUD ---
	var hud = hud_scene.instantiate()
	root.add_child(hud)
	hud.owner = root

	# --- 8. SAVE SCENE ---
	var packed = PackedScene.new()
	var pack_res = packed.pack(root)
	if pack_res != OK:
		printerr("Failed to pack scene! Error: ", pack_res)
		quit(1)
		return

	var save_res = ResourceSaver.save(packed, "res://scenes/level_1.tscn")
	if save_res != OK:
		printerr("Failed to save scene! Error: ", save_res)
		quit(1)
		return

	print("Successfully rebuilt clean level_1.tscn without floating shapes or duplicate connections!")
	orig_root.queue_free()
	root.queue_free()
	quit(0)

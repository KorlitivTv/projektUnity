# Simple Penguin Player Setup

This branch adds a simple controller for the penguin character.

## Use these animations

Use the penguin animation clips you already have:

- `penguin_idle` = normal idle
- `penguin_walk` = normal movement
- `penguin_slide` = sprint/run animation
- `penguin_atack` = attack animation

The character will only have left/right visual direction. The script mirrors the sprite using `SpriteRenderer.flipX`.

## Player GameObject setup

Select your Player object and add these components:

- `Rigidbody2D`
  - Gravity Scale: `0`
  - Freeze Rotation Z: enabled
- `BoxCollider2D`
- `SpriteRenderer`
- `Animator`
- `PenguinPlayerController`

Remove the old `PlayerMovement` script if it is still attached, otherwise both movement scripts may fight each other.

## Animator parameters

Open your Animator Controller and add:

- `Speed` - Float
- `Sprint` - Bool
- `Attack` - Trigger

## Animator states

Drag these animation clips into the Animator window:

- `penguin_idle`
- `penguin_walk`
- `penguin_slide`
- `penguin_atack`

Make `penguin_idle` the default state.

## Transitions

### Idle to Walk

Condition:

- `Speed` greater than `0.01`
- `Sprint` false

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Walk to Idle

Condition:

- `Speed` less than `0.01`

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Walk to Slide/Sprint

Condition:

- `Sprint` true

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Slide/Sprint to Walk

Condition:

- `Sprint` false
- `Speed` greater than `0.01`

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Slide/Sprint to Idle

Condition:

- `Speed` less than `0.01`

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Any State to Attack

Condition:

- `Attack` trigger

Settings:

- Has Exit Time: off
- Transition Duration: 0

### Attack to Idle

Condition:

- none

Settings:

- Has Exit Time: on

## Controls

- WASD / Arrow Keys = move
- Left Shift = sprint/slide animation
- Space = attack

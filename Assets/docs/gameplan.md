# Game design plan or something, idk.
## Theme
### Pixel Art
1 tile = 32x32 pxi
Player = 1.5 tiles tall = 48px*32px
Tilemap: 3x3 for each tile, also consider how tile looks when 3 tiles next to each other, nothing below/above
### Main concept
Guy follows a girl. When he dies, he works out, then gets more buff.


### General
- [ ] HUD - Health bar, Deaths
- [ ] UI - Main menu, Stats per level (time taken, deaths)
- [ ] Working out slideshow after death.
- [ ] Getting rejected slideshow / maybe getting on a date ending.

### Sound
- Background music
- Sound effects:
  - [ ] Jump
  - [ ] Dash
  - [ ] Punch
  - [ ] Getting damaged
  - [ ] Shelf breaking
  - [ ] Homeless guy drinking
  - [ ] Enemy voice line (something for each enemy)
  - [ ] Car sound
  - [ ] Water sound
  - [ ] Ambient sound (city, sewer, mall, warehouse, park)
  - [ ] Rats squeeking
  - [ ] Shopping cart wheels
  - [ ] Turnstile opening
  - [ ] Bouncy sound
  - [ ] Eating sound

### Scenery (backgrounds)
- Street (jumping on roofs, sewer)
- Mall (shelves, escalators, warehouse)
- Park (trees, benches, playground)

### Required assets
- Player sprite (Twig, Normal, Muscular, Buff)
    - Animations:
      - [ ] Idle
      - [ ] Walking/Running 
      - [ ] Wall Sliding 
      - [ ] Jumping-up/falling
      - [ ] Dashing
      - Combat:
        - [ ] Punch: Left jab -> Right jab -> Uppercut
        - [ ] Kick: Jump -> hit
        - [ ] Ground slam: Jump -> down + hit
      - [ ] Damaged animation
- Girl sprite
- Enemy types:
  - [ ] Long distance melee: Attack with purse
  - [ ] Ranged: Throw high heels
  - [ ] Short distance melee: Long nails scratch
  - [ ] Homeless guy: Throws bottles, can be dodged, drinks to heal
  - [ ] Boss: Big lady
    - Ground slam
    - Blitzcrank hook
- Inanimate obstacles:
  - city:
    - [ ] Car (various colors)
    - [ ] Trash can (smell with damage over time)
    - [ ] Fire hydrant (water boosts you upwards)
    - [ ] Slippery floor (Maybe, I dont know)
    - [ ] Rats
    - [ ] Shit pile (smell with damage over time)
    - [ ] Toxic green water (instant kill)
  - mall:
    - [ ] Shopping carts running through
    - [ ] Shelves that fall apart when staying on them for long time
    - [ ] turnstile (turniket)
  - park
    - [ ] Trees
    - [ ] Bouncy trees/bushes
    - [ ] Piknic baskets with healing stuff (maybe animation of eating from it)
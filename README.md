# settlement
  
## ToDo: 
    3. Make peasant. Includes moving movement code from Player.cs to separate class and using it in player and peasant controllers.
    4. Making Bow shop, being able to come up, and spend coins to buy a bow.
    5. Animals and peasant AI controllers. Simple task management for peasant   includes: 
    - pickup coin if it's nearby and become peasant
    - walking around idle
    - pick up bow when there's a bow to pick up
    - try to kill animal if it's around
    6. Make proper data structure for building menu. Now it's just RadialMenuSegmentData which is for display only.
    7. Make tent to act as town center point. Includes asking gamedesigners if we need a town tent.

  ## Patchnotes:

  ### build 0.0.3
    2. Coins dropping and picking up. Decide if we use rigidbody colliders for that or just check position in tile grid. Also some ui representation of how many coins player has. Some code to spawn coins at mouse position with a hotkey for testing purposes.

  ### build 0.0.2
    1. Improve astar pathfinding to support diagonal movement and make it less clunky overall.
    8. Make it possible to move player by holding mouse in pointnclick movement mode.
    10. Wasd camera movement option
    9. Add option to select building spot with mouse after building is selected in Build Menu  
    11. Ingame menu for player settings
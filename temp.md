Player spawn vào StreetScene
  ↓
NPCInteraction (Player) trigger Day2_Scene20_PlayerThought
  → Choices set: choice_confronted_day2 HOẶC choice_ran_away_day2
  → Cả 2 đều set: player_decided_action
  ↓
Player đi vào NPCSurroundController zone
  → requiredFlags: [player_decided_action]
  → NPCs surround → set flag: bullies_surrounded_player
  ↓
NPC Thủ lĩnh detect flag bullies_surrounded_player
  → Conditional 1: requiredFlags [choice_confronted_day2] → Day2_Scene20_Bully_Confrontation
  → Conditional 2: requiredFlags [choice_ran_away_day2] → Day2_Scene20_Bully_RanAway
  ↓
Kết thúc → set flag: day2_scene20_completed
// Matthew Cormack (@johnjoemcbob)
// 27/09/15
//
// A team based gamemode, recreating TF2 class system
//

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using Rust;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using JSONObject = JSON.Object;
using JSONArray = JSON.Array;
using JSONValue = JSON.Value;
using JSONValueType = JSON.ValueType;

namespace Oxide.Plugins
{
	[Info( "Team Fortress 2", "johnjoemcbob", "0.1", ResourceId = -1 )]
	[Description( "Team Fortress 2 style gameplay in Rust." )]
	class TeamFortress2 : RustPlugin
	{
	#region Defines
		private const string PLUGIN_NAME = "Team Fortress 2";
		private const string PLUGIN_AUTHOR = "johnjoemcbob";
		private const string PLUGIN_VERSION = "0.1";
		private const int PLUGIN_ID = -1;
		private const string PLUGIN_DESCRIPTION = "Team Fortress 2 style gameplay in Rust.";

		private const int LOADOUT_ITEM_MAX = 18;
		private const string LOADOUT_ITEM_SHIRT = "teamshirt";
	#endregion

	#region Variables
        private Dictionary<ulong, int> Player_Team; // 64bit Steam ID, Team Index

        // class DynamicContractResolver : DefaultContractResolver
        // {
            // private static bool IsAllowed(JsonProperty property)
            // {
                // return property.PropertyType.IsPrimitive || property.PropertyType == typeof(List<ItemAmount>) ||
                             // property.PropertyType == typeof(ItemAmount[]) ||
                             // property.PropertyType == typeof(List<DamageTypeEntry>) ||
                             // property.PropertyType == typeof(DamageTypeEntry) ||
                             // property.PropertyType == typeof(DamageType) ||
                             // property.PropertyType == typeof(List<ItemModConsumable.ConsumableEffect>) ||
                             // property.PropertyType == typeof(ItemModProjectileRadialDamage) ||
                             // property.PropertyType == typeof(MetabolismAttribute.Type) ||
                             // property.PropertyType == typeof(Rarity) ||
                             // property.PropertyType == typeof(ItemCategory) ||
                             // property.PropertyType == typeof(ItemDefinition) ||
                             // property.PropertyType == typeof(ItemDefinition.Condition) ||
                             // property.PropertyType == typeof(Wearable) ||
                             // property.PropertyType == typeof(Wearable.OccupationSlots) ||
                             // property.PropertyType == typeof(ResourceDispenser.GatherProperties) ||
                             // property.PropertyType == typeof(ResourceDispenser.GatherPropertyEntry) ||
                             // property.PropertyType == typeof(String);
            // }

            // protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            // {
                // var properties = base.CreateProperties(type, memberSerialization);
                // return properties.Where(p => (p.DeclaringType == type || p.DeclaringType == typeof(TimedExplosive) || p.DeclaringType == typeof(BaseMelee)) && IsAllowed(p)).ToList();
            // }
        // }

        // private static JSONObject ToJsonObject(object obj)
        // {
            // return JSONObject.Parse(ToJsonString(obj));
        // }

        // private static JSONArray ToJsonArray(object obj)
        // {
            // return JSONArray.Parse(ToJsonString(obj));
        // }

        // private static string ToJsonString(object obj)
        // {
            // return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                // {
                    // ContractResolver = new DynamicContractResolver(),
                    // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    // Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() }
                // });
        // }
	#endregion

	#region Loadouts
		// There must be an item in the first element of each loadout
		private string[,] Loadout = {
			// Scout
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"hat.cap",
				// Weapons
				"shotgun.pump",
				"pistol.semiauto",
				"bone.club",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"ammo.shotgun.slug",
				"32",
				"",
				"",
				"",
				""
			},
			// Soldier
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"coffeecan.helmet",
				// Weapons
				"rocket.launcher",
				"shotgun.pump",
				"pickaxe",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"ammo.rocket.basic",
				"20",
				"ammo.shotgun",
				"32",
				"",
				""
			},
			// Pyro
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"hazmat.boots",
				"hazmat.pants",
				"hazmat.gloves",
				"hazmat.helmet",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Demoman
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"hat.beenie",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Heavy
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Engineer
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Medic
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Sniper
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			},
			// Spy
			{
				// Clothes
				LOADOUT_ITEM_SHIRT,
				"",
				"",
				"",
				"",
				"",
				// Weapons
				"",
				"",
				"",
				"",
				"",
				"",
				// Misc (Ammo,etc)
				"",
				"",
				"",
				"",
				"",
				""
			}
		};
	#endregion

	#region UI_JSON
		#region UI_Class_Select_JSON
			public string ui_team_select = @"[  
				{
					""_comment"": ""-  -OVERLAY - WINDOW, ALL-  -"",
					""name"": ""Overlay_Team_Select"",
					""parent"": ""Overlay"",
					""components"":
					[
						{
							 ""type"":""UnityEngine.UI.Image"",
							 ""color"":""0.1 0.1 0.1 0.8"",
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0 0"",
							""anchormax"": ""1 1""
						},
						{
							""type"":""NeedsCursor"",
						}
					]
				},
				{
					""_comment"": ""-  -TEXT - TITLE, MIDDLE-TOP-  -"",
					""parent"": ""Overlay_Team_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Text"",
							""text"":""Select a Team!"",
							""fontSize"":30,
							""align"": ""MiddleCenter"",
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.3 0.9"",
							""anchormax"": ""0.7 1.0""
						}
					]
				},
				{
					""_comment"": ""-  -BUTTON - RANDOM, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Team_Select_Button_Random"",
					""parent"": ""Overlay_Team_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.team 3"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.04 0.25"",
							""anchormax"": ""0.24 0.75""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - RANDOM, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Team_Select_Button_Random"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Random"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - SPECTATE, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Team_Select_Button_Spectate"",
					""parent"": ""Overlay_Team_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.team 2"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.28 0.25"",
							""anchormax"": ""0.48 0.75""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - SPECTATE, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Team_Select_Button_Spectate"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Spectate"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - BLU, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Team_Select_Button_BLU"",
					""parent"": ""Overlay_Team_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.team 1"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.52 0.25"",
							""anchormax"": ""0.72 0.75""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - BLU, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Team_Select_Button_BLU"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""BLU"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - RED, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Team_Select_Button_RED"",
					""parent"": ""Overlay_Team_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.team 0"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.76 0.25"",
							""anchormax"": ""0.96 0.75""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - RED, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Team_Select_Button_RED"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""RED"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					}
			]";
			public string ui_class_select = @"[  
				{
					""_comment"": ""-  -OVERLAY - WINDOW, ALL-  -"",
					""name"": ""Overlay_Class_Select"",
					""parent"": ""Overlay"",
					""components"":
					[
						{
							 ""type"":""UnityEngine.UI.Image"",
							 ""color"":""0.1 0.1 0.1 0.8"",
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0 0"",
							""anchormax"": ""1 1""
						},
						{
							""type"":""NeedsCursor"",
						}
					]
				},
				{
					""_comment"": ""-  -TEXT - TITLE, MIDDLE-TOP-  -"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Text"",
							""text"":""Select a Class!"",
							""fontSize"":30,
							""align"": ""MiddleCenter"",
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.3 0.9"",
							""anchormax"": ""0.7 1.0""
						}
					]
				},
				{
					""_comment"": ""-  -BUTTON - SCOUT, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Scout"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 0"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.01 0.45"",
							""anchormax"": ""0.11 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - SCOUT, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Scout"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Scout"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - SOLDIER, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Soldier"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 1"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.12 0.45"",
							""anchormax"": ""0.22 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - SOLDIER, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Soldier"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Soldier"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - PYRO, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Pyro"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 2"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.23 0.45"",
							""anchormax"": ""0.33 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - PYRO, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Pyro"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Pyro"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - DEMOMAN, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Demoman"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 3"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.34 0.45"",
							""anchormax"": ""0.44 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - DEMOMAN, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Demoman"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Demoman"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - HEAVY, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Heavy"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 4"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.45 0.45"",
							""anchormax"": ""0.55 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - HEAVY, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Heavy"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Heavy"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - ENGINEER, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Engineer"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 5"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.56 0.45"",
							""anchormax"": ""0.66 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - ENGINEER, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Engineer"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Engineer"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - MEDIC, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Medic"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 6"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.67 0.45"",
							""anchormax"": ""0.77 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - MEDIC, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Medic"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Medic"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - SNIPER, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Sniper"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 7"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.78 0.45"",
							""anchormax"": ""0.88 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - SNIPER, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Sniper"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Sniper"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - SPY, LEFT-MIDDLE-  -"",
					""name"": ""Overlay_Class_Select_Button_Spy"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class 8"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.89 0.45"",
							""anchormax"": ""0.99 0.55""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - SPY, LEFT-MIDDLE-  -"",
						""parent"": ""Overlay_Class_Select_Button_Spy"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Spy"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					},
				{
					""_comment"": ""-  -BUTTON - CANCEL, MIDDLE-BOTTOM-  -"",
					""name"": ""Overlay_Class_Select_Button_Close"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""command"":""tf2.class_close"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.3 0.15"",
							""anchormax"": ""0.7 0.20""
						}
					]
				},
					{
						""_comment"": ""-  -TEXT - CANCEL, MIDDLE-BOTTOM-  -"",
						""parent"": ""Overlay_Class_Select_Button_Close"",
						""components"":
						[
							{
								""type"":""UnityEngine.UI.Text"",
								""text"":""Close"",
								""fontSize"":24,
								""align"": ""MiddleCenter"",
							},
							{
								""type"":""RectTransform"",
								""anchormin"": ""0.0 0.0"",
								""anchormax"": ""1.0 1.0""
							}
						]
					}
			]";
		#endregion
	#endregion

	#region Functions
		#region Function_Events
			void Loaded()	
			{
				PrintToConsole( "." );
				PrintToConsole( "." );
				PrintToConsole( "." );

				// Refer to item config, attempting to change the number of bullets loaded into weapons
				var gameObjectArray = FileSystem.LoadAll<GameObject>( "Assets/", ".entity" ); // .entity for weapons, .item for other items
				var items = gameObjectArray.Select( x => x.GetComponent<ItemDefinition>() ).Where( x => x != null ).ToList();
				foreach ( var item in gameObjectArray )
				{
					if ( item == null ) continue;

					#region oldtestcode
						// // var ammo = item.GetComponent<ItemModProjectile>();
						// // if ( ammo )
						// // {
							// // var projectile = ammo.projectileObject.Get().GetComponent<Projectile>();
							// // if ( projectile != null )
							// // {
								// // PrintToChat( item.displayName.english );
								// // var mod = ToJsonObject( projectile );
								// // foreach ( var key in mod )
								// // {
									// // PrintToChat( key.ToString() );
								// // }
							// // }
						// // }
						// if (
							// // ( !item.name.ToLower().Contains( "ui." ) ) &&
							// // ( !item.name.ToLower().Contains( "screen" ) ) &&
							// // ( !item.name.ToLower().Contains( "effect" ) ) &&
							// // ( !item.name.ToLower().Contains( ".fx" ) ) &&
							// // ( !item.name.ToLower().Contains( "sound" ) ) &&
							// // ( !item.name.ToLower().Contains( "lootpanel" ) ) &&
							// //( item.name == "thompson.entity" ) ||
							// //( item.name == "rocket_launcher.entity" ) ||
							// //( item.name == "machete.entity" ) ||
							// ( item.name.Contains( "shotgun" ) )
						// )
						// {
							// PrintToConsole( "-" );
							// PrintToConsole( item.name );
							// PrintToConsole( item.GetType().ToString() );
							// PrintToConsole( "-" );
							// var weaponcomponents = item.GetComponents( typeof(Component) );
							// foreach ( var component in weaponcomponents )
							// {
								// string comptype = component.GetType().ToString();
								// // if (
									// // // .item types
									// // ( comptype != "ItemDefinition" ) &&
									// // ( comptype != "ItemBlueprint" ) && // Blueprints
									// // ( comptype != "ItemModEntity" ) && // Traps, Explosives, Deployables, Machete (???)
									// // ( comptype != "ItemModProjectile" ) && // Ammo
									// // ( comptype != "ItemModProjectileRadialDamage" ) && // Explosive 5.56 Rifle Ammo
									// // ( comptype != "ItemModProjectileSpawn" ) && // Special Ammo Types (incendiary,explosive)
									// // ( comptype != "ItemModConsumable" ) && // Food, Drink, Health Items
									// // ( comptype != "ItemModConsume" ) && // Food, Drink, Health Items
									// // ( comptype != "ItemModConsumeContents" ) && // Small Water Bottle only
									// // ( comptype != "ItemModDeployable" ) && // Traps, signs, sleeping bags, tables, etc
									// // ( comptype != "ItemModCookable" ) && // Food, Metals, Oils/Fuels
									// // ( comptype != "ItemModBurnable" ) && // Door Key, Wood
									// // ( comptype != "ItemModWearable" ) && // Clothes
									// // ( comptype != "ItemModSwap" ) && // Human Skull, Wolf Skull
									// // ( comptype != "ItemModMenuOption" ) && // Items with menu options (upgrade,reveal,eat,drink,crush,use,etc)
									// // ( comptype != "ItemModContainer" ) && // Candle Hat, Miners Hat, Small Water Bottle
									// // ( comptype != "ItemModContainerRestriction" ) && // Paper Map only
									// // ( comptype != "ItemModUpgrade" ) && // Blueprint resources before library (Fragment, Page, Book)
									// // ( comptype != "ItemModReveal" ) && // Blueprint resources (Fragment, Page, Book, Library)
									// // // Other types
									// // ( comptype != "UnityEngine.Transform" ) &&
									// // ( comptype != "UnityEngine.BoxCollider" ) &&
									// // ( comptype != "UnityEngine.AudioLowPassFilter" ) &&
									// // ( comptype != "UnityEngine.RigidBody" ) &&
									// // ( comptype != "LootContainer" ) &&
									// // ( comptype != "Spawnable" ) &&
									// // ( comptype != "DecorAlign" ) &&
									// // ( comptype != "DecorRotate" ) &&
									// // ( comptype != "DecorScale" ) &&
									// // ( comptype != "Wearable" ) &&
									// // ( comptype != "TreeEntity" ) &&
									// // ( comptype != "Ragdoll" ) &&
									// // ( comptype != "RagdollInteritable" ) &&
									// // ( comptype != "WorldModel" ) &&
									// // ( comptype != "ScreenBounce" ) &&
									// // ( comptype != "ScreenFov" ) &&
									// // ( comptype != "ScreenRotate" ) &&
									// // ( comptype != "FirstPersonEffect" ) &&
									// // ( comptype != "EffectRecycle" ) &&
									// // ( comptype != "EffectAudioPerspectiveSwitcher" ) &&
									// // ( comptype != "EffectParentToWeaponBone" ) &&
									// // ( comptype != "EffectMuzzleFlash" ) && ///////////////////////////////
									// // ( comptype != "UnparentOnDestroy" ) &&
									// // ( comptype != "FootstepSound" ) &&
									// // ( comptype != "MaxSpawnDistance" ) &&
									// // ( comptype != "FakePhysics" ) &&
									// // ( comptype != "Water" ) &&
									// // ( comptype != "Firebomb" ) &&
									// // ( comptype != "ColliderInfo" ) &&
									// // ( comptype != "BaseMelee" ) && ////////////////////////////////////////
									// // ( comptype != "EntityFlag_Toggle" ) &&
									// // ( comptype != "TorchWeapon" ) &&
									// // ( comptype != "Model" ) &&
									// // ( comptype == "BaseProjectile" ) && /////////////////////////////////////// yessssssssssssssssssssssssssssssssssssssssssssssssssss
									// // ( comptype != "MaxSpawnDistance" )
								// // )
								// {
									// //PrintToConsole( item.displayName.english );
									// PrintToConsole( comptype );
									// if ( comptype == "BaseProjectile" ) //|| comptype == "BaseMelee" || comptype == "BaseLauncher" )
									// {
										// var mod = ToJsonObject( component );
										// PrintToConsole( "---------------" );
										// PrintToConsole( mod.ToString() );
									// }
								// }
							// }
						// }

						// // var weapon = item.GetComponent<ItemModEntity>();
						// // if ( weapon )
						// // {
							// // //PrintToChat( item.displayName.english );
							// // if ( item.displayName.english == "Machete" )
							// // {
								// // var weaponcomponents = weapon.entityPrefab.Get().GetComponents( typeof(Component) );
								// // foreach ( var component in weaponcomponents )
								// // {
									// // //PrintToChat( component.GetType().ToString() );
								// // }
								// // // if ( weaponent != null )
								// // // {
									// // // PrintToChat( item.displayName.english );
									// // // var mod = ToJsonObject( weaponent );
									// // // foreach ( var key in mod )
									// // // {
										// // // PrintToChat( key.ToString() );
									// // // }
								// // // }
							// // }
						// // }
					#endregion
				}
			}

			// void OnServerInitialized()
			// {
				// Player_Team = new Dictionary<ulong, int>();
			// }

			// void OnPlayerConnected( BasePlayer player )
			// {
				// // Assign to spectator first
				// Player_Team[player.userID] = 2;

				// // The bring up the team choice UI
				// //UI_Team_Select( player );
			// }

			// void OnEntityTakeDamage( BaseCombatEntity entity, HitInfo info )
			// {
				// // Both entities are players
				// if ( ( entity is BasePlayer ) && ( info.Initiator is BasePlayer ) )
				// {
					// BasePlayer victim = entity as BasePlayer;
					// BasePlayer attacker = info.Initiator as BasePlayer;

					// // One of the players is spectating, or they are on the same team; negate damage
					// if (
						// ( Player_Team[victim.userID] == 2 ) ||
						// ( Player_Team[attacker.userID] == 2 ) ||
						// ( Player_Team[victim.userID] == Player_Team[attacker.userID] )
					// )
					// {
						// info.damageTypes.ScaleAll( 0 );
						// return;
					// }
				// }
				// return;
			// }

			// void OnTick()
			// {
				// // Close the UI if the user exits it with a button press
				// // if ( input.IsDown( BUTTON.INVENTORY ) )
				// // {
					// // CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select" );
				// // }
			// }

			void OnPlayerRespawned( BasePlayer player )
			{
				Class_Select( player, 0 );
			}

			void OnPlayerSleepEnded( BasePlayer player )
			{
				Class_Select( player, 0 );
			}
		#endregion

		#region Function_Team
			void Team_Select( BasePlayer player, int team )
			{
				if ( ( team < 0 ) || ( team >= 4 ) ) return;

			}

			void UI_Team_Select( BasePlayer player )
			{
				// Send the formatted JSON UI to the server
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "AddUI", ui_team_select );
			}

			void UI_Team_Select_Close( BasePlayer player )
			{
				// Close the menu
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Team_Select" );
				// Must also close the button text seperately because they are parented to the buttons (?)
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Team_Select_Button_Random" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Team_Select_Button_Spectate" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Team_Select_Button_BLU" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Team_Select_Button_RED" );
			}
		#endregion

		#region Function_Class
			void Class_Select( BasePlayer player, int loadout )
			{
				if ( ( loadout < 0 ) || ( loadout >= 9 ) ) return;

				// First wipe the player of all old items
				player.inventory.Strip();

				// Give the player the items of the loadout
				if ( Loadout[loadout, 0] != "" )
				{
					int itemnum = 0;
					while ( itemnum < LOADOUT_ITEM_MAX )
					{
						if ( Loadout[loadout, itemnum] != "" )
						{
							string itemname = Loadout[loadout, itemnum];
								if ( itemname == LOADOUT_ITEM_SHIRT )
								{
									if ( Player_Team.ContainsKey( player.userID ) )
									{
										// Change shirt to be team coloured
										if ( Player_Team[player.userID] == 1 )
										{
											itemname = "hoodie";
										}
										else
										{
											itemname = "tshirt.long";
										}
									}
									else
									{
										continue;
									}
								}
							var item = ItemManager.FindItemDefinition( itemname );
							// Place the loadout item in the correct inventory container
							int itemcount = 1;
							var container = player.inventory.containerWear;
								if ( itemnum >= 12 )
								{
									container = player.inventory.containerMain;
									// Loadout for ammo has the next element as the number to give
									itemcount = Convert.ToInt32( Loadout[loadout, itemnum + 1] );
								}
								else if ( itemnum >= 6 )
								{
									container = player.inventory.containerBelt;
								}
							player.inventory.GiveItem( ItemManager.CreateByItemID( (int) item.itemid, itemcount, false ), container );
						}

						// Increment through loadout
						itemnum++;
						// If the item is ammo, it also has a number so increment again
						if ( itemnum > 12 )
						{
							itemnum++;
						}
					}
				}

				// Lock the player's inventory
				player.inventory.containerWear.SetFlag( ItemContainer.Flag.IsLocked, true );
				player.inventory.containerBelt.SetFlag( ItemContainer.Flag.IsLocked, true );
				player.inventory.containerMain.SetFlag( ItemContainer.Flag.IsLocked, true );

				// var json = ToJsonObject( player.inventory.containerBelt.GetSlot( 0 ).GetHeldEntity().gameObject );
				// PrintToConsole( "--------------------------------" );
				// PrintToConsole( "--------------------------------" );
				// PrintToConsole( json.ToString() );
				// PrintToConsole( "--------------------------------" );
				// PrintToConsole( "--------------------------------" );

				// // get all public static methods of MyClass type
				// MethodInfo[] methodInfos = player.GetType().GetMethods(); //player.inventory.containerBelt.GetSlot( 0 ).GetType().GetMethods();
				// PrintToConsole( player.inventory.containerBelt.GetType().ToString() );
				// foreach ( MethodInfo info in methodInfos )
				// {
					// PrintToConsole( info.Name );
				// }

				// var worldent = player.inventory.containerBelt.GetSlot( 0 ).GetHeldEntity() as BaseProjectile;
				// worldent.projectileAmount = 100;
				// worldent.reloadTime = 0.1f;
			}

			void UI_Class_Select( BasePlayer player )
			{
				// Send the formatted JSON UI to the server
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "AddUI", ui_class_select );
			}

			void UI_Class_Select_Close( BasePlayer player )
			{
				// Close the menu
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select" );
				// Must also close the button text seperately because they are parented to the buttons (?)
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Scout" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Soldier" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Pyro" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Demoman" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Heavy" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Engineer" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Medic" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Sniper" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Spy" );
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Close" );
			}
		#endregion
	#endregion

	#region ConsoleCommands
		[ConsoleCommand( "tf2.team_close" )]
		void Plugin_Command_Team_Select_Close( ConsoleSystem.Arg arg )
		{
			// Ensure there was a player sending the command
            if ( arg.connection == null ) return;

			// Close the menu
            BasePlayer player = arg.connection.player as BasePlayer;
			UI_Team_Select_Close( player );
		}

		[ConsoleCommand( "tf2.team" )]
		void Plugin_Command_Team_Select( ConsoleSystem.Arg arg )
		{
			// Ensure the class arguement is present, and there was a player sending the command
            if ( ( arg.Args == null ) || ( arg.Args.Length < 1 ) ) return;
            if ( arg.connection == null ) return;

            BasePlayer player = arg.connection.player as BasePlayer;

			// Select the new team
			int teamselect = 0;
			if ( int.TryParse( arg.Args[0], out teamselect ) )
			{
				Team_Select( player, teamselect );
			}

			// Close the menu
			UI_Team_Select_Close( player );
		}

		[ConsoleCommand( "tf2.class_close" )]
		void Plugin_Command_Class_Select_Close( ConsoleSystem.Arg arg )
		{
			// Ensure there was a player sending the command
            if ( arg.connection == null ) return;

			// Close the menu
            BasePlayer player = arg.connection.player as BasePlayer;
			UI_Class_Select_Close( player );
		}

		[ConsoleCommand( "tf2.class" )]
		void Plugin_Command_Class_Select( ConsoleSystem.Arg arg )
		{
			// Ensure the class arguement is present, and there was a player sending the command
            if ( ( arg.Args == null ) || ( arg.Args.Length < 1 ) ) return;
            if ( arg.connection == null ) return;

            BasePlayer player = arg.connection.player as BasePlayer;

			// Select the new class
			int classselect = 0;
			if ( int.TryParse( arg.Args[0], out classselect ) )
			{
				Class_Select( player, classselect );
			}

			// Close the menu
			UI_Class_Select_Close( player );
		}
	#endregion

	#region ChatCommands
		// Display plugin help in chat
		[ChatCommand( "tf2" )]
		void Plugin_Chat_DisplayHelp( BasePlayer player, string cmd, string[] args )
		{
			//var json = ToJsonObject( player.inventory.containerBelt.GetSlot( 0 ).GetHeldEntity() );
			// ( player.GetActiveItem().GetHeldEntity() as BaseProjectile ).projectileVelocityScale = 0.01f;
			// player.GetActiveItem().condition = 50;
			// var json = ToJsonObject( player.GetActiveItem().GetHeldEntity() as BaseEntity );
			// PrintToConsole( "--------------------------------" );
			// PrintToConsole( "--------------------------------" );
			// PrintToConsole( json.ToString() );
			// PrintToConsole( "--------------------------------" );
			// PrintToConsole( "--------------------------------" );

			PrintToChat( "Test" );
		}

		// Displays the team loadout screen
		[ChatCommand( "team" )]
		void Plugin_Chat_DisplayTeamSelect( BasePlayer player, string cmd, string[] args )
		{
			// if ( args.Length > 0 )
			// {
				// // Skip the menu and immediately change
				// int classselect = 0;
				// if ( int.TryParse( args[0], out classselect ) )
				// {
					// Class_Select( player, classselect );
					// return;
				// }
			// }
			UI_Team_Select( player );
		}

		// Displays the class loadout screen
		[ChatCommand( "class" )]
		void Plugin_Chat_DisplayClassSelect( BasePlayer player, string cmd, string[] args )
		{
			if ( args.Length > 0 )
			{
				// Skip the menu and immediately change
				int classselect = 0;
				if ( int.TryParse( args[0], out classselect ) )
				{
					Class_Select( player, classselect );
					return;
				}
			}
			UI_Class_Select( player );
		}
	#endregion
	}
}

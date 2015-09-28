// Matthew Cormack (@johnjoemcbob)
// 27/09/15
//
// A team based gamemode, recreating TF2 class system
//

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Rust;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;

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
				
			}

			void OnTick()
			{
				// Close the UI if the user exits it with a button press
				// if ( input.IsDown( BUTTON.INVENTORY ) )
				// {
					// CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select" );
				// }
			}

			void OnPlayerRespawned( BasePlayer player )
			{
				Class_Select( player, 0 );
			}

			void OnPlayerSleepEnded( BasePlayer player )
			{
				Class_Select( player, 0 );
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
									// Change shirt to be team coloured
									if ( UnityEngine.Random.Range( 1, 2 ) == 1 )
									{
										itemname = "hoodie";
									}
									else
									{
										itemname = "tshirt.long";
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
				CommunityEntity.ServerInstance.ClientRPCEx( new Network.SendInfo() { connection = player.net.connection }, null, "DestroyUI", "Overlay_Class_Select_Button_Close" );
			}
		#endregion
	#endregion

	#region ConsoleCommands
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
			PrintToChat( "Test" );
			Class_Select( player, 0 );
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

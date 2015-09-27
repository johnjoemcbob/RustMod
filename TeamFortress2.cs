// Matthew Cormack (@johnjoemcbob)
// 27/09/15
//
// A team based gamemode, recreating TF2 class system
//

using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Rust;

namespace Oxide.Plugins
{
	[Info( "Team Fortress 2", "johnjoemcbob", "0.1", ResourceId = -1 )]
	[Description( "Team Fortress 2 style gameplay in Rust." )]
	class TeamFortress2 : RustPlugin
	{
	#region Defines
		private string PLUGIN_NAME = "Team Fortress 2";
		private string PLUGIN_AUTHOR = "johnjoemcbob";
		private string PLUGIN_VERSION = "0.1";
		private int PLUGIN_ID = -1;
		private string PLUGIN_DESCRIPTION = "Team Fortress 2 style gameplay in Rust.";

		private int LOADOUT_ITEM_MAX = 12;
	#endregion

	#region Loadouts
		// There must be an item in the first element of each loadout
		private string[,] Loadout = {
			// Scout
			{
				// Clothes
				"hazmat.jacket",
				"",
				"",
				"",
				"",
				"hat.beenie",
				// Weapons
				"shotgun.pump",
				"pistol.semiauto",
				"bone.club",
				"",
				"",
				""
			},
			// Soldier
			{
				// Clothes
				"",
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
				""
			},
			// Pyro
			{
				// Clothes
				"",
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
				""
			},
			// Demoman
			{
				// Clothes
				"",
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
				""
			},
			// Heavy
			{
				// Clothes
				"",
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
				""
			},
			// Engineer
			{
				// Clothes
				"",
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
				""
			},
			// Medic
			{
				// Clothes
				"",
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
				""
			},
			// Sniper
			{
				// Clothes
				"",
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
				""
			},
			// Spy
			{
				// Clothes
				"",
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
				""
			}
		};
	#endregion

	#region UI_JSON
		#region UI_Class_Select_JSON
			public string ui_class_select = @"[  
				{
					""_comment"": ""-  -This element is the main overlay, and enables the cursor-  -"",
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
					""_comment"": ""-  -This element is the title of the overlay, located at the top-center of the screen-  -"",
					""name"": ""Overlay_Class_Select_Title"",
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
					""_comment"": ""-  -This element is the cancel button which exits the class selection overlay, located at the bottom-left of the screen-  -"",
					""name"": ""Overlay_Class_Select_Button_Close"",
					""parent"": ""Overlay_Class_Select"",
					""components"":
					[
						{
							""type"":""UnityEngine.UI.Button"",
							""close"":""Overlay_Class_Select"",
							""color"": ""0.5 0.5 0.5 0.2"",
							""imagetype"": ""Tiled""
						},
						{
							""type"":""RectTransform"",
							""anchormin"": ""0.3 0.15"",
							""anchormax"": ""0.5 0.20""
						}
					]
				},
				{
					""_comment"": ""-  -This element is the title of the overlay, located at the top-center of the screen-  -"",
					""name"": ""Overlay_Class_Select_Button_Close_Text"",
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
							var item = ItemManager.FindItemDefinition( Loadout[loadout, itemnum] );
							var container = player.inventory.containerWear;
								if ( itemnum >= 6 )
								{
									container = player.inventory.containerBelt;
								}
							player.inventory.GiveItem( ItemManager.CreateByItemID( (int) item.itemid, 1, false ), container );
						}

						itemnum++;
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
		#endregion
	#endregion

	#region Commands
		// Display plugin help in chat
		[ChatCommand( "tf2" )]
		void Plugin_DisplayHelp( BasePlayer player, string cmd, string[] args )
		{
			PrintToChat( "Test" );
			Class_Select( player, 0 );
		}

		// Displays the class loadout screen
		[ChatCommand( "class" )]
		void Plugin_DisplayClassSelect( BasePlayer player, string cmd, string[] args )
		{
			UI_Class_Select( player );
		}
	#endregion
	}
}

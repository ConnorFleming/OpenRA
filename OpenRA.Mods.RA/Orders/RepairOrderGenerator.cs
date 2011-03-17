﻿#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made 
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.RA.Buildings;
using OpenRA.Traits;

namespace OpenRA.Mods.RA.Orders
{
	class RepairOrderGenerator : IOrderGenerator
	{
		public IEnumerable<Order> Order(World world, int2 xy, MouseInput mi)
		{
			if (mi.Button == MouseButton.Right)
				world.CancelInputMode();

			return OrderInner(world, xy, mi);
		}

		IEnumerable<Order> OrderInner(World world, int2 xy, MouseInput mi)
		{
			if (mi.Button == MouseButton.Left)
			{
				var underCursor = world.FindUnitsAtMouse(mi.Location)
					.Where(a => a.Owner == world.LocalPlayer && a.HasTrait<RepairableBuilding>()).FirstOrDefault();

				if (underCursor == null)
					yield break;
				
				if (underCursor.Info.Traits.Contains<RepairableBuildingInfo>()
				    && underCursor.GetDamageState() > DamageState.Undamaged)
					yield return new Order("Repair", underCursor, false);
			}
		}

		public void Tick( World world )
		{
			if( !PlayerIsAllowedToRepair( world ) )
				world.CancelInputMode();
		}

		public static bool PlayerIsAllowedToRepair( World world )
		{		
			return world.Queries.WithTrait<AllowsBuildingRepair>()
                .Any(a => a.Actor.Owner == world.LocalPlayer);
		}

		public void RenderAfterWorld( WorldRenderer wr, World world ) { }
		public void RenderBeforeWorld( WorldRenderer wr, World world ) { }

		public string GetCursor(World world, int2 xy, MouseInput mi)
		{
			mi.Button = MouseButton.Left;
			return OrderInner(world, xy, mi).Any() 
				? "repair" : "repair-blocked";
		}
	}
}

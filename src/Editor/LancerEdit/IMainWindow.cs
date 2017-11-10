﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2017
 * the Initial Developer. All Rights Reserved.
 */
using System;
using Xwt;
namespace LancerEdit
{
	public interface IMainWindow : LibreLancer.IUIThread
	{
		void AddTab(LTabPage page);
		void RemoveTab(LTabPage page);
		LTabPage GetCurrentTab();
		void Quit();
		Xwt.Drawing.Image GetImage(byte[] data, int width, int height);
		ISortableList ConstructList();
	}
}
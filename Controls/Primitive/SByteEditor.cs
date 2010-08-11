/*
    Reflexil .NET assembly editor.
    Copyright (C) 2007-2010 Sebastien LEBRETON

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#region " Imports "
using System;
using Mono.Cecil.Cil;
#endregion

namespace Reflexil.Editors
{

    public partial class SByteEditor : BaseSByteEditor
	{
		
		#region " Methods "
		public override Instruction CreateInstruction(ILProcessor worker, OpCode opcode)
		{
			return worker.Create(opcode, SelectedOperand);
		}
		#endregion
		
	}

    #region " VS Designer generic support "
    public class BaseSByteEditor : GenericOperandEditor<SByte>
    {
        public override Instruction CreateInstruction(ILProcessor worker, OpCode opcode)
        {
            throw new NotImplementedException();
        }
    }
    #endregion	
	
}


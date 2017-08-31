using UnityEngine;
using System.Collections.Generic;

public struct RowCol {
	public int row;
	public int col;

	public RowCol(int setRor, int setCol) {
		row = setRor;
		col = setCol;
	}

	public RowCol(RowCol rc) {
		row = rc.row;
		col = rc.col;
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To make folding icons persistent, place this line in the settings file.
// "editor.showFoldingControls": "always"

/* Design */

/* Couplings:
*   Squares
* State:
*   OnDisplay:
*     MouseOverSquare
*     HighlightedSquares:
*       Clicked
*       Source - such as a piece
*       Destination - such as the perimeter of an advancement square
*       Presentation - such as Rook planes
* Responsibilities:
*   Contains the set of squares
*   Support queries on Properties & state
*   Support commands to change state
*/

/* Graphical:
 * Square separation grid setting (1)
 * Vertical separation between levels (should be multiple of squares horizontal separation)
 */

public class ChessBoard3D : MonoBehaviour
{
	ChessBoard3D() { print("ChessBoard3D.ctor()"); }
	~ChessBoard3D() { print("ChessBoard3D.dtor()"); }

	public ChessBoardProperties chessBoardProperties;

	public Vector3Int size = new Vector3Int(0, 0, 0);

    // Assumes square size is 1.0 or less; typically 0.90.
    public GameObject whiteSquare;
    public GameObject blackSquare;

    public GameObject[,,] squares;

	// Set from ChessBoardProperties.
	private int boardVerticalSep;
	private float squareSep;
	void Start()
    {
		print("----- ChessBoard3D.Start() -----");
		chessBoardProperties.squareSize = whiteSquare.transform.localScale.x;
		chessBoardProperties.squareThickness = whiteSquare.transform.localScale.y;

		boardVerticalSep = chessBoardProperties.boardVerticalSep;
		squareSep = chessBoardProperties.squareSep;

		print("  Initial board.size = " + chessBoardProperties.size);
		print("  squareSize = " + chessBoardProperties.squareSize);
		print("  squareThickness = " + chessBoardProperties.squareThickness);
		print("  boardVerticalSep = " + boardVerticalSep);
		print("  squareSep = " + squareSep);
	}

	void Update() {}

	// New method more compliant with SRP.
	public void CreateStandardBoard(Vector3Int size, Vector3Int locWhiteK11sq)
	{
		print("ChessBoard3D.CreateStandardBoard() of " + size.x + "x" + size.y + "x" + size.z + " with level sep " + boardVerticalSep);

		squares = new GameObject[size.x, size.y, size.z];

		this.size = chessBoardProperties.size = size;
		chessBoardProperties.locWhiteK11sq = locWhiteK11sq;
		bool firstSqIsWhite = locWhiteK11sq.z % 2 == 0;

		chessBoardProperties.ComputeBoardEdges();

		Vector2 boardXedges = chessBoardProperties.boardXedges;
		Vector2 boardYedges = chessBoardProperties.boardYedges;
		Vector2 boardZedges = chessBoardProperties.boardZedges;

		for (int k = 0; k < size.z; k++) // Build each level.
		{
			for (int j = 0; j < size.y; j++) // Build each column.
			{
				for (int i = 0; i < size.x; i++) // build each row.
				{
					if ((i + j + k) % 2 == (firstSqIsWhite ? 0 : 1)) {
						squares[i, j, k] = Instantiate(whiteSquare);
					} else {
						squares[i, j, k] = Instantiate(blackSquare);
					}
					squares[i, j, k].transform.position = new Vector3(boardYedges[0] + squareSep / 2 + j, boardZedges[0] + k * boardVerticalSep, boardXedges[0] + squareSep / 2 + i);

					// Alternate levels for black/grey squares.
					if (k % 2 == (firstSqIsWhite ? 0 : 1) && (i + j) % 2 == 1) {
						MeshRenderer aMesh = squares[i, j, k].GetComponent<MeshRenderer>();
						aMesh.materials[0].SetColor("_Color", Color.gray);
					}
				}
			}
		}

		// Mark White K11 square in blue.
		MeshRenderer myMesh = squares[chessBoardProperties.locWhiteK11sq.x, chessBoardProperties.locWhiteK11sq.y, chessBoardProperties.locWhiteK11sq.z].GetComponent<MeshRenderer>();
		myMesh.materials[0].SetColor("_Color", Color.blue);
	}
}

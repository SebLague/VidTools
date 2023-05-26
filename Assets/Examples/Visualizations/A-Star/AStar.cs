using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis;
using UnityEngine.InputSystem;

namespace VidTools.Examples
{
	public class AStar : MonoBehaviour
	{
		[Header("Settings")]
		public Vector2Int gridSize;
		public Vector2Int startCoord;
		public Vector2Int targetCoord;

		[Header("Display Settings")]
		[Range(0.9f, 1)]
		public float tileSize = 1;
		public bool allowDiagonals;
		public Color emptyTileCol;
		public Color obstacleTileCol;
		public Color pathTileCol;
		public Color pathLineCol;
		public Color backgroundCol;
		public float pathThickness;

		Tile[,] tiles;
		Vector2 mousePressOld;
		Tile TargetNode => tiles[targetCoord.x, targetCoord.y];
		bool hasSolution;

		void Update()
		{
			HandleObstaclePlacementInput();
			if (!hasSolution)
			{
				ResetTiles();
				Solve();
				hasSolution = true;
			}
			DrawVis();


		}

		void Solve()
		{
			Tile startNode = tiles[startCoord.x, startCoord.y];
			Tile targetNode = tiles[targetCoord.x, targetCoord.y];

			List<Tile> tilesToExplore = new() { startNode };

			while (tilesToExplore.Count > 0)
			{
				tilesToExplore.Sort((a, b) => a.totalCost.CompareTo(b.totalCost));
				Tile currentNode = tilesToExplore[0];
				tilesToExplore.RemoveAt(0);
				currentNode.alreadyVisisted = true;

				if (currentNode == targetNode)
				{
					Debug.Log("Solved");
					return;
				}

				foreach (Tile neighbour in currentNode.neighbours)
				{
					if (neighbour.alreadyVisisted)
					{
						continue;
					}

					if (!CanTraverse(currentNode, neighbour)) { continue; }

					int movementCostToNeighbour = currentNode.distanceFromStart + GetDistance(currentNode, neighbour) + TurnCost(currentNode, neighbour);
					if (movementCostToNeighbour < neighbour.distanceFromStart || !tilesToExplore.Contains(neighbour))
					{
						neighbour.distanceFromStart = movementCostToNeighbour;
						neighbour.estimatedDistanceToTarget = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						if (!tilesToExplore.Contains(neighbour))
						{
							tilesToExplore.Add(neighbour);
						}
					}
				}
			}

			Debug.Log("Failed to find path");
		}

		int TurnCost(Tile from, Tile to)
		{
			if (from.parent == null)
			{
				return 0;
			}
			Vector2 currDir = ((Vector2)(from.coord - from.parent.coord)).normalized;
			Vector2 newDir = ((Vector2)(to.coord - from.coord)).normalized;
			float change = 1 - Vector2.Dot(currDir, newDir);
			return Mathf.CeilToInt(change * 5);
		}

		bool CanTraverse(Tile from, Tile to)
		{
			if (!to.Walkable)
			{
				return false;
			}

			// Don't allow crossing between diagonally placed obstacles
			if (from.coord.y != to.coord.y && from.coord.x != to.coord.y)
			{
				if (!CoordIsWalkable(from.coord + Vector2Int.up) && !CoordIsWalkable(to.coord + Vector2Int.down))
				{
					return false;
				}
				if (!CoordIsWalkable(from.coord + Vector2Int.down) && !CoordIsWalkable(to.coord + Vector2Int.up))
				{
					return false;
				}
			}
			return true;
		}

		bool CoordIsWalkable(Vector2Int coord)
		{
			if (coord.x < 0 || coord.x >= gridSize.x || coord.y < 0 || coord.y >= gridSize.y)
			{
				return false;
			}
			return tiles[coord.x, coord.y].Walkable;
		}

		int GetDistance(Tile nodeA, Tile nodeB)
		{
			int dstX = Mathf.Abs(nodeA.coord.x - nodeB.coord.x);
			int dstY = Mathf.Abs(nodeA.coord.y - nodeB.coord.y);

			if (dstX > dstY)
				return 14 * dstY + 10 * (dstX - dstY);
			return 14 * dstX + 10 * (dstY - dstX);
		}



		void Init()
		{
			hasSolution = false;
			if (tiles == null || tiles.GetLength(0) != gridSize.x || tiles.GetLength(1) != gridSize.y)
			{
				tiles = new Tile[gridSize.x, gridSize.y];
				// Init nodes
				for (int y = 0; y < gridSize.y; y++)
				{
					for (int x = 0; x < gridSize.x; x++)
					{
						Vector2 pos = new Vector2(-(gridSize.x - 1) / 2f + x, -(gridSize.y - 1) / 2f + y);
						tiles[x, y] = new Tile() { coord = new Vector2Int(x, y), position = pos };
					}
				}

				// Set neighbours
				for (int y = 0; y < gridSize.y; y++)
				{
					for (int x = 0; x < gridSize.x; x++)
					{
						tiles[x, y].neighbours = new List<Tile>();

						for (int offsetY = -1; offsetY <= 1; offsetY++)
						{
							for (int offsetX = -1; offsetX <= 1; offsetX++)
							{
								if (offsetX == 0 && offsetY == 0) { continue; }
								if (offsetX != 0 && offsetY != 0 && !allowDiagonals) { continue; }
								Vector2Int neighbour = new Vector2Int(x + offsetX, y + offsetY);
								if (neighbour.x < 0 || neighbour.x >= gridSize.x || neighbour.y < 0 || neighbour.y >= gridSize.y) { continue; }
								tiles[x, y].neighbours.Add(tiles[neighbour.x, neighbour.y]);
							}
						}
					}
				}
			}

		}

		void ResetTiles()
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					tiles[x, y].parent = null;
					tiles[x, y].distanceFromStart = 0;
					tiles[x, y].estimatedDistanceToTarget = 0;
					tiles[x, y].alreadyVisisted = false;
				}
			}
		}

		void HandleObstaclePlacementInput()
		{
			Mouse mouse = Mouse.current;
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

			if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.isPressed)
			{
				mousePressOld = mousePos;
			}

			if (mouse.leftButton.isPressed || mouse.rightButton.isPressed)
			{
				bool placing = mouse.leftButton.isPressed;
				const int iterations = 5;
				for (int i = 0; i < iterations; i++)
				{
					Vector2 pos = Vector2.Lerp(mousePressOld, mousePos, i / (iterations - 1f));
					float tx = (pos.x + gridSize.x / 2f) / gridSize.x;
					float ty = (pos.y + gridSize.y / 2f) / gridSize.y;

					if (tx >= 0 && tx < 1 && ty >= 0 && ty < 1)
					{
						int x = (int)(tx * gridSize.x);
						int y = (int)(ty * gridSize.y);
						if (tiles[x, y].obstacle != placing)
						{
							hasSolution = false;
							tiles[x, y].obstacle = placing;
						}
					}
				}
				mousePressOld = mousePos;
			}

		}

		void DrawVis()
		{
			HashSet<Tile> solutionNodes = new HashSet<Tile>();
			Tile solutionNode = TargetNode;
			List<Vector2> solutionPath = new List<Vector2>();

			while (solutionNode.parent != null)
			{
				if (solutionNode != TargetNode)
				{
					solutionPath.Add(solutionNode.position);
				}
				solutionNodes.Add(solutionNode);
				solutionNode = solutionNode.parent;
			}


			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					Color col = tiles[x, y].obstacle ? obstacleTileCol : emptyTileCol;
					col = emptyTileCol;

					if (solutionNodes.Contains(tiles[x, y]))
					{
						col = pathTileCol;
					}
					if (x == startCoord.x && y == startCoord.y)
					{
						col = Color.green;
					}
					if (x == targetCoord.x && y == targetCoord.y)
					{
						col = Color.yellow;
					}
					Draw.Quad(tiles[x, y].position, Vector2.one * tileSize, col);
				}
			}

			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					Tile node = tiles[x, y];
					if (node.obstacle)
					{
						node.animT += Time.deltaTime * 4;
						Draw.Quad(tiles[x, y].position, Vector2.one * tileSize * Ease.Quadratic.InOut(node.animT), obstacleTileCol);
					}
				}
			}

			Draw.Path(solutionPath.ToArray(), pathThickness, false, pathLineCol);

			Camera.main.backgroundColor = backgroundCol;
		}

		void OnValidate()
		{
			gridSize = new Vector2Int(Mathf.Max(0, gridSize.x), Mathf.Max(0, gridSize.y));
			startCoord = new Vector2Int(Mathf.Clamp(startCoord.x, 0, gridSize.x - 1), Mathf.Clamp(startCoord.y, 0, gridSize.y - 1));
			targetCoord = new Vector2Int(Mathf.Clamp(targetCoord.x, 0, gridSize.x - 1), Mathf.Clamp(targetCoord.y, 0, gridSize.y - 1));

			if (Application.isPlaying)
			{
				Init();
			}
		}

		class Tile
		{
			public bool obstacle;
			public int distanceFromStart;
			public int estimatedDistanceToTarget;
			public int totalCost => distanceFromStart + estimatedDistanceToTarget;
			public Tile parent;
			public List<Tile> neighbours;

			public Vector2Int coord;
			public Vector2 position;
			public bool alreadyVisisted;
			public float animT;

			public bool Walkable => !obstacle;
		}
	}
}
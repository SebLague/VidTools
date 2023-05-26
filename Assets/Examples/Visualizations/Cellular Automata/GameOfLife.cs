using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis;
using UnityEngine.InputSystem;

namespace VidTools.Examples
{
	public class GameOfLife : MonoBehaviour
	{

		public Vector2Int gridSize;
		public bool diagonalNeighbours = true;
		public float timeBetweenUpdates = 0.1f;
		[Range(0.9f, 1)]
		public float tileSize = 1;
		public Color deadTileCol;
		public Color aliveTileCol;
		public Color backgroundCol;
		Vector2 mousePressOld;

		bool simulating;

		Cell[,] cells;
		float lastSimUpdateTime;

		void Start()
		{
			Debug.Log("Left click to place. Press space to simulate.");
		}


		void Update()
		{
			Init();
			HandleDrawInput();
			HandleSimControlInput();
			Simulate();
			DrawGame();
		}

		void Simulate()
		{
			float timeSinceLastUpdate = Time.time - lastSimUpdateTime;
			if (!simulating || timeSinceLastUpdate < timeBetweenUpdates) { return; }

			lastSimUpdateTime = Time.time;
			bool[,] nextState = new bool[gridSize.x, gridSize.y];
			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					Cell cell = cells[x, y];
					nextState[x, y] = cell.alive;

					int numLiveNeighbours = 0;
					foreach (Cell neighbour in cell.neighbours)
					{
						if (neighbour.alive)
						{
							numLiveNeighbours++;
						}
					}

					// Rules (note, these can be condensed, but writing out in full for clarity...)
					if (cell.alive)
					{
						if (numLiveNeighbours < 2)
						{
							nextState[x, y] = false; // Underpopulation
						}
						else if (numLiveNeighbours > 3)
						{
							nextState[x, y] = false; // Overpopulation
						}
					}
					else
					{
						if (numLiveNeighbours == 3)
						{
							nextState[x, y] = true; // Reproduction
						}
					}
				}
			}

			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					Cell cell = cells[x, y];
					cell.alive = nextState[x, y];
				}
			}
		}

		void Init()
		{
			if (cells == null || cells.GetLength(0) != gridSize.x || cells.GetLength(1) != gridSize.y)
			{
				cells = new Cell[gridSize.x, gridSize.y];
				// Init nodes
				for (int y = 0; y < gridSize.y; y++)
				{
					for (int x = 0; x < gridSize.x; x++)
					{
						cells[x, y] = new Cell();
					}
				}

				// Set neighbours
				for (int y = 0; y < gridSize.y; y++)
				{
					for (int x = 0; x < gridSize.x; x++)
					{
						cells[x, y].neighbours = new List<Cell>();

						for (int offsetY = -1; offsetY <= 1; offsetY++)
						{
							for (int offsetX = -1; offsetX <= 1; offsetX++)
							{
								if (offsetX == 0 && offsetY == 0) { continue; }
								if (offsetX != 0 && offsetY != 0 && !diagonalNeighbours) { continue; }
								Vector2Int neighbour = new Vector2Int(x + offsetX, y + offsetY);
								if (neighbour.x < 0 || neighbour.x >= gridSize.x || neighbour.y < 0 || neighbour.y >= gridSize.y) { continue; }
								cells[x, y].neighbours.Add(cells[neighbour.x, neighbour.y]);
							}
						}
					}
				}
			}
		}

		void HandleSimControlInput()
		{
			if (Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				simulating = !simulating;

			}
		}
		void HandleDrawInput()
		{
			Mouse mouse = Mouse.current;
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

			if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
			{
				mousePressOld = mousePos;
			}

			if (mouse.leftButton.isPressed || mouse.rightButton.isPressed)
			{
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
						cells[x, y].alive = mouse.leftButton.isPressed;
					}
				}
				mousePressOld = mousePos;
			}

		}

		void DrawGame()
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				for (int x = 0; x < gridSize.x; x++)
				{
					Cell tile = cells[x, y];
					Color col = tile.alive ? aliveTileCol : deadTileCol;
					Vector2 pos = new Vector2(-(gridSize.x - 1) / 2f + x, -(gridSize.y - 1) / 2f + y);
					Draw.Quad(pos, Vector2.one * tileSize, col);
				}
			}

			Camera.main.backgroundColor = backgroundCol;
		}

		class Cell
		{
			public bool alive;
			public List<Cell> neighbours;
		}
	}
}
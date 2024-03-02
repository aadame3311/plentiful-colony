using System;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using Godot;
using Godot.Collections;

public class PoissonDiscSampling
{
    private Array<Vector2I> _grid;
    private Array<Vector2I> _activeList;
    private WorldData _worldData;
    private float _cellSize;
    private int _numCols;
    private int _numRows;
    private int _maxTryLimit;
    private int _sampleGridIndex;
    private float _radius;


    public PoissonDiscSampling(double radius, int maxTryLimit, WorldData worldData)
    {
        _radius = (float)radius;
        _maxTryLimit = maxTryLimit;
        _cellSize = _radius / Mathf.Sqrt(2);
        _worldData = worldData;

        _numCols = Mathf.FloorToInt(worldData.worldDimmensions.X / _cellSize);
        _numRows = Mathf.FloorToInt(worldData.worldDimmensions.Y / _cellSize);

        _numCols -= 1;

        _grid = new Array<Vector2I>(new Vector2I[_numCols * _numRows]);
        _grid.Fill(new Vector2I(-1, -1));
    }

    public Array<Vector2I> Process()
    {
        var startPoint = new Vector2I(_worldData.worldDimmensions.X / 2, _worldData.worldDimmensions.Y / 2);
        Array<int> indexes = _ToGrid(startPoint);

        var gridStartPointIndex = indexes[1] + indexes[0] * _numCols;
        _grid[gridStartPointIndex] = startPoint;
        _activeList = new Array<Vector2I>
        {
            startPoint
        };

        while (_activeList.Count > 0)
        {
            Vector2I activePoint = _activeList.PickRandom();
            Vector2I sample = GenerateSample(activePoint);

            if (sample.X != -1 && sample.Y != -1)
            {
                _grid[_sampleGridIndex] = sample;
                _activeList.Add(sample);
            }
            else
            {
                _activeList.Remove(activePoint);
            }
        }

        return _grid;
    }

    public Vector2I GenerateSample(Vector2I activePoint)
    {
        for (int i = 0; i < _maxTryLimit; i++)
        {
            // generate random point between r and 2r radius from active point
            float randomAngle = (float)GD.RandRange(0, 2 * Mathf.Pi);
            var randomDistance = Mathf.FloorToInt(GD.RandRange(_radius, 2 * _radius));
            var sample = new Vector2I(
                Mathf.FloorToInt(activePoint.X + randomDistance * Mathf.Cos(randomAngle)),
                Mathf.FloorToInt(activePoint.Y + randomDistance * Mathf.Sin(randomAngle))
            );

            var sampleGrid = _ToGrid(sample);
            _sampleGridIndex = sampleGrid[1] + sampleGrid[0] * _numCols;

            if (_sampleGridIndex <= 0 || _sampleGridIndex >= _grid.Count || sampleGrid[0] < 0 || sampleGrid[1] < 0 || sampleGrid[0] > _numRows || sampleGrid[1] > _numCols || _grid[_sampleGridIndex].X != -1 && _grid[_sampleGridIndex].Y != -1)
            {
                continue;
            }

            // check if at least r distance away from 8 neighboring point
            if (_IsNeighborOk(sampleGrid[0], sampleGrid[1], sample))
            {
                return sample;
            }

        }

        return new Vector2I(-1, -1);
    }

    private bool _IsNeighborOk(int gridI, int gridJ, Vector2I sample)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                var checkingIndex = (gridJ + j) + (gridI + i) * _numCols - 1;
                // GD.Print($"Checking index {gridJ} {j} {gridI} {i} -- {checkingIndex} -- {_grid.Count}");
                if (checkingIndex >= 0 && checkingIndex < _grid.Count && _grid[checkingIndex].X != -1 && _grid[checkingIndex].Y != -1)
                {
                    Vector2 test = new Vector2(0, 0);

                    float distanceSqr = new Vector2(sample.X, sample.Y).DistanceSquaredTo(_grid[checkingIndex]);
                    if (distanceSqr < _radius)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private Array<int> _ToGrid(Vector2I position)
    {
        return new Array<int> { Mathf.FloorToInt(position.Y / _cellSize), Mathf.FloorToInt(position.X / _cellSize) };
    }
}
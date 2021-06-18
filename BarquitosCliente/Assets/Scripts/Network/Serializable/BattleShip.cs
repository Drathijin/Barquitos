using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

public class BattleShip : ISerializable
{
    int size = 2;

    public bool horizontal = true;

    public struct Position
    {
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
    }

    List<Position> placedPositions_;

    public BattleShip(int size)
    {
        placedPositions_ = new List<Position>();
        this.size = size;
        size_ = (uint)(sizeof(int) + 1 + (sizeof(int) * 2 * 5));
    }

    public List<Position> PlacedPositions()
    {
        return placedPositions_;
    }

    public void AddPlacedPosition(int x, int y)
    {
        placedPositions_.Add(new Position(x, y));
    }

    public bool CheckAttack(int x, int y)
    {
        for (int i = 0; i < placedPositions_.Count; i++)
        {
            if (placedPositions_[i].x == x && placedPositions_[i].y == y)
            {
                placedPositions_.RemoveAt(i);
                break;
            }
        }
        return Destroyed();
    }

    public bool Destroyed()
    {
        return placedPositions_.Count <= 0;
    }

    public int Size()
    {
        return size;
    }

    override public Byte[] ToBin()
    {
        int posCount = placedPositions_.Count;
        if (posCount > 5 || posCount < 2 || size != posCount)
        {
            throw new Exception("Inconsistent battleship size\n");
        }

        data_ = new Byte[size_];

        var shipSize = BitConverter.GetBytes(size);
        var hor = BitConverter.GetBytes(horizontal);

        shipSize.CopyTo(data_, 0);
        hor.CopyTo(data_, sizeof(int));

        const int offset = sizeof(int) + 1;

        for (int i = 0; i < posCount; i++)
        {
            var x_ = BitConverter.GetBytes(placedPositions_[i].x);
            var y_ = BitConverter.GetBytes(placedPositions_[i].y);
            x_.CopyTo(data_, offset + sizeof(int) * i * 2);
            y_.CopyTo(data_, offset + sizeof(int) * i * 2 + sizeof(int));
        }

        if (BitConverter.IsLittleEndian)
            Array.Reverse(data_);
        return data_;
    }
    override public void FromBin(Byte[] data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);
        data_ = data;

        size = BitConverter.ToInt32(data, 0);
        if (size > 5 || size < 2)
        {
            throw new Exception("Inconsistent battleship size\n");
        }

        horizontal = BitConverter.ToBoolean(data, sizeof(int));
        int offset = sizeof(int) + 1;
        placedPositions_ = new List<Position>();
        for (int i = 0; i < size; i++)
        {
            int x = BitConverter.ToInt32(data_, offset + sizeof(int) * i * 2);
            int y = BitConverter.ToInt32(data_, offset + sizeof(int) * i * 2 + sizeof(int));
            Position pos = new Position(x, y);
            placedPositions_.Add(pos);
        }
    }
}

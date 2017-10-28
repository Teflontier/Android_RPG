using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappedTile<T> {

    public Tile tile;
    public T metadata;

    public WrappedTile(Tile tile, T metadata) {
        this.tile = tile;
        this.metadata = metadata;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {

    public PieceType type;
    private Piece currentPiece;

    public void Spawn() {
        int amountObjects = 0;
        switch(type) {
            case PieceType.jump:
                amountObjects = LevelManager.Instance.jumps.Count;
                break;
            case PieceType.slide:
                amountObjects = LevelManager.Instance.slides.Count;

                break;
            case PieceType.longblock:
                amountObjects = LevelManager.Instance.longblocks.Count;

                break;
            case PieceType.ramp:
                amountObjects = LevelManager.Instance.ramps.Count;
                break;
        }

        currentPiece = LevelManager.Instance.GetPiece(type, Random.Range(0, amountObjects));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void Despawn() {
        currentPiece.gameObject.SetActive(false);
    }
}

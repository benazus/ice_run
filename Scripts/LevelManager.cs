using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public bool showCollider = false; // Colliders for debug $$

    public static LevelManager Instance { set; get; }

    // Level Spawning
    private const float DISTANCE_BEFORE_SPAWN = 100f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continuousSegments;
    private int currentSpawnZ;
    private int currentLevelOfElevation; // 0: ground, 1: elevated
    private int y1, y2, y3;

    // Lists of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();

    [HideInInspector]
    public List<Piece> pieces = new List<Piece>();

    // Lists of segments
    public List<Segment> avaiableSegments = new List<Segment>();
    public List<Segment> avaiableTransitions = new List<Segment>();

    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    // Gameplay
    private bool isMoving = false;

    private void Awake() {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevelOfElevation = 0;
    }

    private void Start() {
        for (int i = 0; i < INITIAL_SEGMENTS; i++) {
            if (i < INITIAL_TRANSITION_SEGMENTS)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }

    private void Update() {
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN) {
            GenerateSegment();

            if(amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN) {
                segments[amountOfActiveSegments - 1].DeSpawn(); // Because degments are ordered
                amountOfActiveSegments--;
            }
        }
    }

    private void GenerateSegment() {
        SpawnSegment();

        if(Random.Range(0f, 1f) < (continuousSegments * 0.25f)) {
            // Spawn transition
            continuousSegments = 0;
            SpawnTransition();
        }
        else {
            continuousSegments++;
        }

        continuousSegments++;
    }

    private void SpawnSegment() {
        List<Segment> possibleSegments = avaiableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSegments.Count);

        Segment s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;
        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition() {
        List<Segment> possibleTransition = avaiableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;
        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition) {
        Segment s = null;

        s = segments.Find(x => x.SegId == id 
            && x.transition == transition && x.gameObject.activeSelf == false);

        if(s == null) {
            GameObject go = Instantiate(
                (transition) ? avaiableTransitions[id].gameObject : 
                avaiableSegments[id].gameObject
                ) as GameObject;

            s = go.GetComponent<Segment>();
            s.SegId = id;
            s.transition = transition;
            segments.Insert(0, s);
        }
        else {
            segments.Remove(s);
            segments.Insert(0, s); // To keep track of which segment has been spawned last, and which can be removed first
        }

        return s;
    }

    public Piece GetPiece(PieceType pt, int visualIndex) {
        Piece p = pieces.Find(x => x.type == pt 
            && x.visualIndex == visualIndex && x.gameObject.activeSelf == false);

        if (p == null) {
            GameObject go = null;
            switch (pt) {
                case PieceType.ramp:
                    go = ramps[visualIndex].gameObject;
                    break;
                case PieceType.longblock:
                    go = longblocks[visualIndex].gameObject;
                    break;
                case PieceType.jump:
                    go = jumps[visualIndex].gameObject;
                    break;
                case PieceType.slide:
                    go = slides[visualIndex].gameObject;
                    break;
            }
            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
        }

        return p;
    }
}
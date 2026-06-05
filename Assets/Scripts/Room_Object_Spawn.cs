using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Room_Object_Spawn : MonoBehaviour
{
    [Header("MRUK")]
    [SerializeField] private MRUK.RoomFilter spawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;
    [SerializeField] private bool trackAnchorUpdates = true;

    [Header("Door Spawn")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 localEulerOffset = Vector3.zero;

    private readonly Dictionary<MRUKAnchor, GameObject> _spawnedDoors = new();
    private readonly HashSet<MRUKRoom> _registeredRooms = new();

    private void Start()
    {
        if (MRUK.Instance == null)
        {
            Debug.LogWarning($"{nameof(Room_Object_Spawn)} requires an active MRUK instance in the scene.", this);
            return;
        }

        MRUK.Instance.RegisterSceneLoadedCallback(HandleSceneLoaded);
        MRUK.Instance.RoomCreatedEvent.AddListener(HandleRoomCreated);
        MRUK.Instance.RoomRemovedEvent.AddListener(HandleRoomRemoved);
    }

    private void HandleSceneLoaded()
    {
        if (spawnOnStart == MRUK.RoomFilter.None)
        {
            return;
        }

        RegisterTrackedRooms();
        RespawnDoors();
    }

    private void HandleRoomCreated(MRUKRoom room)
    {
        if (!ShouldTrackRoom(room))
        {
            return;
        }

        RegisterRoomCallbacks(room);
        SpawnDoorsInRoom(room);
    }

    private void HandleRoomRemoved(MRUKRoom room)
    {
        UnregisterRoomCallbacks(room);
        ClearSpawnedDoors(room);
    }

    private void HandleAnchorCreated(MRUKAnchor anchor)
    {
        if (!ShouldTrackRoom(anchor?.Room))
        {
            return;
        }

        TrySpawnDoor(anchor);
    }

    private void HandleAnchorUpdated(MRUKAnchor anchor)
    {
        if (anchor == null)
        {
            return;
        }

        if (!ShouldTrackRoom(anchor.Room) || !IsDoorAnchor(anchor))
        {
            ClearSpawnedDoor(anchor);
            return;
        }

        TrySpawnDoor(anchor);
    }

    private void HandleAnchorRemoved(MRUKAnchor anchor)
    {
        ClearSpawnedDoor(anchor);
    }

    [ContextMenu("Respawn Doors")]
    public void RespawnDoors()
    {
        if (MRUK.Instance == null)
        {
            Debug.LogWarning($"{nameof(Room_Object_Spawn)} cannot respawn doors because MRUK is not available.", this);
            return;
        }

        if (doorPrefab == null)
        {
            Debug.LogWarning($"{nameof(Room_Object_Spawn)} needs a Door Prefab assigned.", this);
            ClearSpawnedDoors();
            return;
        }

        ClearSpawnedDoors();

        switch (spawnOnStart)
        {
            case MRUK.RoomFilter.CurrentRoomOnly:
                SpawnDoorsInRoom(MRUK.Instance.GetCurrentRoom());
                break;
            case MRUK.RoomFilter.AllRooms:
                foreach (var room in MRUK.Instance.Rooms)
                {
                    SpawnDoorsInRoom(room);
                }
                break;
        }
    }

    [ContextMenu("Clear Spawned Doors")]
    public void ClearSpawnedDoors()
    {
        foreach (var spawnedDoor in _spawnedDoors.Values)
        {
            if (spawnedDoor)
            {
                Destroy(spawnedDoor);
            }
        }

        _spawnedDoors.Clear();
    }

    private void ClearSpawnedDoors(MRUKRoom room)
    {
        if (room == null)
        {
            return;
        }

        List<MRUKAnchor> anchorsToRemove = new();
        foreach (var pair in _spawnedDoors)
        {
            if (pair.Key == null || pair.Key.Room != room)
            {
                continue;
            }

            if (pair.Value)
            {
                Destroy(pair.Value);
            }

            anchorsToRemove.Add(pair.Key);
        }

        foreach (var anchor in anchorsToRemove)
        {
            _spawnedDoors.Remove(anchor);
        }
    }

    private void ClearSpawnedDoor(MRUKAnchor anchor)
    {
        if (anchor == null || !_spawnedDoors.TryGetValue(anchor, out var spawnedDoor))
        {
            return;
        }

        if (spawnedDoor)
        {
            Destroy(spawnedDoor);
        }

        _spawnedDoors.Remove(anchor);
    }

    private void RegisterTrackedRooms()
    {
        if (!trackAnchorUpdates || MRUK.Instance == null)
        {
            return;
        }

        foreach (var room in MRUK.Instance.Rooms)
        {
            if (ShouldTrackRoom(room))
            {
                RegisterRoomCallbacks(room);
            }
        }
    }

    private void RegisterRoomCallbacks(MRUKRoom room)
    {
        if (room == null || !_registeredRooms.Add(room))
        {
            return;
        }

        room.AnchorCreatedEvent.AddListener(HandleAnchorCreated);
        room.AnchorUpdatedEvent.AddListener(HandleAnchorUpdated);
        room.AnchorRemovedEvent.AddListener(HandleAnchorRemoved);
    }

    private void UnregisterRoomCallbacks(MRUKRoom room)
    {
        if (room == null || !_registeredRooms.Remove(room))
        {
            return;
        }

        room.AnchorCreatedEvent.RemoveListener(HandleAnchorCreated);
        room.AnchorUpdatedEvent.RemoveListener(HandleAnchorUpdated);
        room.AnchorRemovedEvent.RemoveListener(HandleAnchorRemoved);
    }

    private void SpawnDoorsInRoom(MRUKRoom room)
    {
        if (room == null)
        {
            return;
        }

        foreach (var anchor in room.Anchors)
        {
            TrySpawnDoor(anchor);
        }
    }

    private void TrySpawnDoor(MRUKAnchor anchor)
    {
        if (!IsDoorAnchor(anchor) || doorPrefab == null)
        {
            return;
        }

        if (!_spawnedDoors.TryGetValue(anchor, out var spawnedDoor) || !spawnedDoor)
        {
            spawnedDoor = Instantiate(doorPrefab, anchor.transform);
            spawnedDoor.name = $"{doorPrefab.name}_{anchor.Label}";
            _spawnedDoors[anchor] = spawnedDoor;
        }

        AlignSpawnedDoor(anchor, spawnedDoor.transform);
    }

    private void AlignSpawnedDoor(MRUKAnchor anchor, Transform spawnedTransform)
    {
        spawnedTransform.SetParent(anchor.transform, false);
        spawnedTransform.localPosition = GetDoorLocalCenter(anchor) + localPositionOffset;
        spawnedTransform.localRotation = Quaternion.Euler(localEulerOffset);
    }

    private static Vector3 GetDoorLocalCenter(MRUKAnchor anchor)
    {
        if (anchor.VolumeBounds.HasValue)
        {
            return anchor.VolumeBounds.Value.center;
        }

        if (anchor.PlaneRect.HasValue)
        {
            var center = anchor.PlaneRect.Value.center;
            return new Vector3(center.x, center.y, 0f);
        }

        return Vector3.zero;
    }

    private bool ShouldTrackRoom(MRUKRoom room)
    {
        if (room == null || MRUK.Instance == null)
        {
            return false;
        }

        return spawnOnStart switch
        {
            MRUK.RoomFilter.AllRooms => true,
            MRUK.RoomFilter.CurrentRoomOnly => room == MRUK.Instance.GetCurrentRoom(),
            _ => false,
        };
    }

    private static bool IsDoorAnchor(MRUKAnchor anchor)
    {
        return anchor != null && anchor.HasAnyLabel(MRUKAnchor.SceneLabels.DOOR_FRAME);
    }

    private void OnDestroy()
    {
        if (MRUK.Instance != null)
        {
            MRUK.Instance.SceneLoadedEvent.RemoveListener(HandleSceneLoaded);
            MRUK.Instance.RoomCreatedEvent.RemoveListener(HandleRoomCreated);
            MRUK.Instance.RoomRemovedEvent.RemoveListener(HandleRoomRemoved);
        }

        foreach (var room in _registeredRooms)
        {
            if (room == null)
            {
                continue;
            }

            room.AnchorCreatedEvent.RemoveListener(HandleAnchorCreated);
            room.AnchorUpdatedEvent.RemoveListener(HandleAnchorUpdated);
            room.AnchorRemovedEvent.RemoveListener(HandleAnchorRemoved);
        }

        _registeredRooms.Clear();
        ClearSpawnedDoors();
    }
}

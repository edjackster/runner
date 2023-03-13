using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField]
    private Transform _playerPosition;

    [SerializeField]
    private Chunk[] _chunkPrefabs;

    [SerializeField]
    private Chunk _firstChunk;

    private List<Chunk> _spawnedChunks = new List<Chunk>();

    // Start is called before the first frame update
    void Start()
    {
        _spawnedChunks.Add(_firstChunk);
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerPosition.position.x> _spawnedChunks[_spawnedChunks.Count - 1].End.position.x - 50)
        {
            spawnChunk();
        }
    }

    private void spawnChunk()
    {
        Chunk chunk = Instantiate(_chunkPrefabs[Random.Range(0, _chunkPrefabs.Length)]);
        chunk.transform.position = _spawnedChunks[_spawnedChunks.Count-1].End.position - chunk.Begin.localPosition;
        _spawnedChunks.Add(chunk);

        if (-_spawnedChunks.Count > 2)
        {
            Destroy(_spawnedChunks[0].gameObject);
            _spawnedChunks.RemoveAt(0);
        }
    }
}

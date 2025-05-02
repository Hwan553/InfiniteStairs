using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    public GameManager _gameManager;
    public GameObject _kiwiPrefab;

    GameObject[] _kiwi;
    GameObject[] _targetPool;
    void Awake()
    {
        _kiwi = new GameObject[20];
        Generate();
        StartCoroutine(SpawnKiwis());
    }


    void Generate()
    {
        for(int i = 0; i < _kiwi.Length; i++)
        {
            _kiwi[i] = Instantiate(_kiwiPrefab, _gameManager.Stairs[i].transform);
            _kiwi[i].transform.localPosition = new Vector3(0, 0.6f, 0);
            _kiwi[i].SetActive(false);
            
        }
    }

    public void MakeObj(string type, int count)
    {
        switch (type)
        {
            case "_kiwi":
                _targetPool = _kiwi;
                break;
        }

        List<int> Indexes = new List<int>();

       
        for (int i = 0; i < _targetPool.Length; i++)
        {
            if (!_targetPool[i].activeSelf)
            {
                Indexes.Add(i);
            }
        }

        
        int spawnCount = Mathf.Min(count, Indexes.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, Indexes.Count);
            _targetPool[Indexes[randomIndex]].SetActive(true);
            Indexes.RemoveAt(randomIndex); 
        }
    }
    IEnumerator SpawnKiwis()
    {
        while (true)
        {
            MakeObj("_kiwi", Random.Range(1, 3));
            yield return new WaitForSeconds(Random.Range(3f, 7f)); 
        }
    }

    
    
}

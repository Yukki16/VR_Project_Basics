using UnityEngine;

public class SimpleInteractableButton : MonoBehaviour
{
    [SerializeField] GameObject dartPrefab;

    public void OnHover()
    {
        Vector3 newSpawn = this.transform.position + new Vector3(0, 1, 0);
        Instantiate(dartPrefab, newSpawn, Quaternion.identity);
    }
}

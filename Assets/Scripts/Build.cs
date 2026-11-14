using UnityEngine;

public class Build : MonoBehaviour
{
    public GameObject prefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject power = Instantiate(prefab);

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                prefab.transform.position = hit.point;
            }
        }
    }
}
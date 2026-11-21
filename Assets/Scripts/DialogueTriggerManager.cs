using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTriggerManager : MonoBehaviour
{
    [SerializeField] private string npcTag = "NPC";

    private Transform selectedNPC;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            if (selection.CompareTag(npcTag))
            {
                Debug.Log(selection.tag);
                selectedNPC = selection;
            }
        }

    }
}

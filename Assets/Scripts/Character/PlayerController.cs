using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    const float offsetY = 0.3f;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private Vector2 input;

    private Character character;

    private void Awake(){
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove Diagonal Movement
        
            if (input.x != 0) input.y = 0; 

            if (input != Vector2.zero)
            {
               StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTrainersView();
    }

    private void CheckForEncounters(){
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.GrassLayer) != null){
            if (UnityEngine.Random.Range(1, 101) <= 10){
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name {
        get => name;
    }

    public Sprite Sprite {
        get => sprite;
    }
}

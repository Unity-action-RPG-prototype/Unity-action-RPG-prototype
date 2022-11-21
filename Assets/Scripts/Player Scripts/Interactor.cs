using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _interactor;
    [SerializeField] InputControl _input;
    [SerializeField] GameObject _assassinateUI;
    [SerializeField] Animator _animator;
    [SerializeField] AnimationScript _animation;
    [SerializeField] PlayerMovement _playerMovement;

    private bool assassinate;

    public LayerMask InteractionLayers;
    public Vector3 playerDirection;
    public bool isInteractable;
    public float InteractableDistance;

    RaycastHit interactable;
    private void Awake()
    {
        _assassinateUI.SetActive(false);
    }

    private void FixedUpdate()
    {
        isInteractable = Physics.Raycast(_interactor.transform.position, _camera.transform.forward * 20f, out interactable, 20f, InteractionLayers); 
    }

    public void Update()
    {
        if (isInteractable)
        {
            InteractableDistance = Vector3.Distance(transform.position, interactable.collider.transform.position);
        }

        //assassination interaction
        if (isInteractable
            && interactable.collider.gameObject.layer == LayerMask.NameToLayer("Assassination Interaction")
            && InteractableDistance <= 0.5f
            )
        {
            Debug.DrawRay(_interactor.transform.position, _camera.transform.forward * 20f, Color.cyan, 0.5f);
            _assassinateUI.SetActive(true);
            //Debug.Log(interactable.collider.name);
            var target = interactable.collider.gameObject;
            var targetHealth = target.GetComponentInParent<EnemyHealth>();
            var targetMovement = target.GetComponentInParent<EnemyAI>();

            if (
                _input.assassinate
                && !assassinate
                && target != null
                && targetMovement.currentState != EnemyAI.STATE.COMBAT
                )
            {
                StartCoroutine(Assassinate(targetHealth));
            }
        }
        else
        {
            Debug.DrawRay(_interactor.transform.position, _camera.transform.forward * 20f, Color.magenta, 0.5f);
            _assassinateUI.SetActive(false);
        }
    }

    IEnumerator Assassinate(EnemyHealth enemyHealth)
    {
        _input.canMove = false;
        assassinate = true;
        enemyHealth.Assassinated();
        Debug.Log("isdead");
        _animator.SetTrigger(_animation.assassinate);
        yield return new WaitForSeconds(5f);
        assassinate = false;
        _input.canMove = true;
    }


}

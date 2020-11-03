using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    public float _maxSpeed = 1.0f;
    public float speed = 1.0f;
    public float distance_wall = 5.0f;
    public float wall_speed = 0;
    public float Mass = 15;
    public float player_Hp = 5;

    public float timer_dead;

    private bool _isArriveOn = true;
    public bool _isDead = false;

    public Vector3 _velocity = Vector3.zero;
    private Vector3 wall_velocity = Vector3.zero;
    private Vector3 deadZone = Vector3.zero;
    private Vector3 respawnZone = Vector3.zero;
    private Vector3 _pickPos;
    private Vector3 _pickPos_2;

    private RaycastHit hit;
    private RaycastHit hit_2;
    private RaycastHit hit_3;
    private RaycastHit hit_4;
    private RaycastHit hit_5;

    public LayerMask m_layerMask = -1;

    private void Start()
    {
        deadZone = new Vector3(90.0f, 0.0f, -40.0f);
        respawnZone = new Vector3(-90.0f, 0.0f, 40.0f);

        _pickPos = transform.position;
        _pickPos_2 = transform.position;
    }

    private void Update()
    {
        timer_dead += Time.deltaTime;

        if (player_Hp == 0)
        {
            _isDead = true;
            timer_dead = 0.0f;

            transform.position = deadZone;

            player_Hp = 5;
        }
        if (_isDead == true && timer_dead >= 3.0f)
        {
            _isDead = false;

            transform.position = respawnZone;
            _pickPos = transform.position;
            _pickPos_2 = transform.position;
        }

        if (_isDead == false)
        {
            if (Input.GetKeyUp(KeyCode.CapsLock))
            {
                _isArriveOn = !_isArriveOn;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouse_pos = Input.mousePosition;

                if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
                {
                    _pickPos = hit.point;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouse_pos_2 = Input.mousePosition;

                if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos_2), -Vector3.up, out hit_2, 1000))
                {
                    _pickPos_2 = hit_2.point;

                    _velocity = _velocity + (Flee(_pickPos_2));
                }
            }

            Wall_Avoidance();

            Vector3 targetVelocity = _pickPos - transform.position;

            if (!_isArriveOn)
            {
                _velocity = _velocity + (Seek(_pickPos) * Time.deltaTime);
            }
            else
            {
                _velocity = _velocity + (Arrive(_pickPos) * Time.deltaTime);

                if (targetVelocity.magnitude < 1.0f)
                {
                    _velocity = Vector3.zero;
                }
            }

            if (_velocity != Vector3.zero)
            {
                transform.position = transform.position + _velocity;
                transform.forward = _velocity.normalized;
            }
        }
        else
        {
            Debug.Log("리스폰까지 : " + (3.0f - timer_dead));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_pickPos_2, 1.0f);
    }

    private Vector3 Wall_Avoidance()
    {
        hit_3.distance = distance_wall;
        hit_4.distance = distance_wall;
        hit_5.distance = distance_wall;

        Ray raycast_forward = new Ray();
        Ray raycast_right = new Ray();
        Ray raycast_left = new Ray();

        raycast_forward.origin = transform.position;
        raycast_right.origin = transform.position;
        raycast_left.origin = transform.position;

        raycast_forward.direction = transform.forward;
        raycast_right.direction = transform.forward + transform.right;
        raycast_left.direction = transform.forward - transform.right;

        Debug.DrawLine(transform.position, transform.position + raycast_forward.direction * hit_3.distance * 3, Color.red);
        Debug.DrawLine(transform.position, transform.position + raycast_right.direction * hit_4.distance, Color.red);
        Debug.DrawLine(transform.position, transform.position + raycast_left.direction * hit_5.distance, Color.red);

        if (Physics.Raycast(raycast_right, out hit_4, hit_4.distance))
        {
            if (hit_4.collider.tag == "Wall" || hit_4.collider.tag == "Tower")
            {
                if (Physics.Raycast(raycast_forward, out hit_3, hit_3.distance * 3))
                    wall_speed = ((raycast_forward.direction * hit_3.distance * 3) - hit_3.transform.position).magnitude;

                wall_velocity = hit_4.normal * wall_speed / Mass;

                wall_velocity.y = 0;

                _velocity = _velocity + wall_velocity;
            }
        }
        else if (Physics.Raycast(raycast_left, out hit_5, hit_5.distance))
        {
            if (hit_5.collider.tag == "Wall" || hit_5.collider.tag == "Tower")
            {
                if (Physics.Raycast(raycast_forward, out hit_3, hit_3.distance * 3))
                    wall_speed = ((raycast_forward.direction * hit_3.distance * 3) - hit_3.transform.position).magnitude;

                wall_velocity = hit_5.normal * wall_speed / Mass;

                wall_velocity.y = 0;

                _velocity = _velocity + wall_velocity;
            }
        }

        return _velocity;
    }

    private Vector3 Seek(Vector3 target_pos)
    {
        Vector3 desired_velocity_1 = ((target_pos - transform.position).normalized) * _maxSpeed;

        desired_velocity_1.y = 0.0f;

        return (desired_velocity_1 - _velocity);
    }

    private Vector3 Flee(Vector3 target_pos)
    {
        Vector3 desired_velocity_2 = ((transform.position - target_pos).normalized) * _maxSpeed;

        desired_velocity_2.y = 0.0f;

        return (desired_velocity_2 - _velocity);
    }

    private Vector3 Arrive(Vector3 target_pos)
    {
        Vector3 targetVelocity = target_pos - transform.position;

        targetVelocity.y = 0.0f;

        float dist = targetVelocity.magnitude;

        if (dist < 1.0f)
        {
            _velocity = Vector3.zero;
            return _velocity;
        }

        if (dist > 40.0f)
        {
            speed = _maxSpeed;
        }
        else
        {
            speed = _maxSpeed * (dist / 40.0f);
        }

        targetVelocity.Normalize();
        targetVelocity *= speed;

        Vector3 acceleration = targetVelocity - _velocity;

        acceleration *= 1 / 0.1f;

        if (acceleration.magnitude > 15.0f)
        {
            acceleration.Normalize();
            acceleration *= 5.0f;
        }

        speed = Mathf.Min(speed, _maxSpeed);

        return acceleration;
    }
}

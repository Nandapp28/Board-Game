using System.IO.Ports;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    private SerialPort serialPort;
    public string portName = "COM3"; // Change this to the correct port (e.g., COM3 on Windows or /dev/ttyUSB0 on Linux/Mac)
    public int baudRate = 115200;

    // Variables to store joystick and button values
    private int joystickX, joystickY, buttonState;

    void Start()
    {
        // Initialize serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        serialPort.ReadTimeout = 1;  // Set read timeout to 1ms to avoid blocking the program
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                // Read a line of data from the serial port
                string data = serialPort.ReadLine();
                string[] values = data.Split(',');

                // Parse the joystick and button values
                joystickX = int.Parse(values[0]);
                joystickY = int.Parse(values[1]);
                buttonState = int.Parse(values[2]);

                // Use joystick data for movement or interaction (example)
                float moveX = joystickX / 4095f * 10f;  // Map to -5 to 5
                float moveY = joystickY / 4095f * 10f;  // Map to -5 to 5
                transform.position = new Vector3(moveX, moveY, 0f);

                // Handle button press (example)
                if (buttonState == 0)
                {
                    Debug.Log("Button Pressed!");
                }
                else
                {
                    Debug.Log("Button Not Pressed!");
                }

                // Set threshold for detecting movement
                int threshold = 100; // Adjust this value as needed

                // Debugging arah gerakan dengan threshold
                if (joystickX > 2048 + threshold) // Gerakan ke kanan
                {
                    Debug.Log("Gerakan: Kanan");
                }
                else if (joystickX < 2048 - threshold) // Gerakan ke kiri
                {
                    Debug.Log("Gerakan: Kiri");
                }

                if (joystickY > 2048 + threshold) // Gerakan ke atas
                {
                    Debug.Log("Gerakan: Atas");
                }
                else if (joystickY < 2048 - threshold) // Gerakan ke bawah
                {
                    Debug.Log("Gerakan: Bawah");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Close the serial port when the application quits
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}

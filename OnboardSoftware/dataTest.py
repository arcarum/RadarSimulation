import websocket
import json
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import threading

fig, ax = plt.subplots(figsize=(10, 10), subplot_kw=dict(projection='polar'))
im = None
cbar = None

def create_ppi_plot(data, azimuth, range_bins, vmin=None, vmax=None):
    global im, cbar
    
    # Convert polar coordinates to cartesian
    theta = np.radians(azimuth)
    r, theta = np.meshgrid(range_bins, theta)
    
    # Plot the data
    if im is None:
        im = ax.pcolormesh(theta, r, data, cmap='viridis', vmin=vmin, vmax=vmax)
        
        # Customize the plot
        ax.set_theta_zero_location("N")
        ax.set_theta_direction(-1)
        ax.set_rlabel_position(0)
        ax.set_title("PPI Plot")
        
        # Add a colorbar
        cbar = plt.colorbar(im, ax=ax)
        cbar.set_label('Intensity')
    else:
        im.set_array(data.ravel())
        im.set_clim(vmin=vmin, vmax=vmax)
    
    return im

latest_data = None
data_lock = threading.Lock()

def on_message(ws, message):
    global latest_data
    data = json.loads(message)
    ppi = data.get('PPI', 'NA')
    if ppi == "NA":
        return
    ppi = np.array(ppi)
    
    with data_lock:
        latest_data = ppi

def on_error(ws, error):
    print(f"Error: {error}")

def on_close(ws, close_status_code, close_msg):
    print("Connection closed")

def on_open(ws):
    print("Connection opened")

def run_websocket():
    ws = websocket.WebSocketApp("ws://localhost:8080/data",
                                on_message=on_message,
                                on_error=on_error,
                                on_close=on_close,
                                on_open=on_open)
    ws.run_forever()

def update_plot(frame):
    global latest_data
    with data_lock:
        if latest_data is not None:
            num_azimuth, num_range = latest_data.shape
            azimuth = np.linspace(0, 360, num_azimuth)
            range_bins = np.linspace(0, num_range, num_range)
            
            return create_ppi_plot(latest_data, azimuth, range_bins, vmin=latest_data.min(), vmax=latest_data.max())

if __name__ == "__main__":
    # Start WebSocket connection in a separate thread
    websocket_thread = threading.Thread(target=run_websocket)
    websocket_thread.daemon = True
    websocket_thread.start()

    # Set up the animation
    ani = FuncAnimation(fig, update_plot, interval=100, blit=False)
    plt.show()
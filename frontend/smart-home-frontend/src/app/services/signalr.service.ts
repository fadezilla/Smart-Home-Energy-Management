import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hub/energy`) // Matches your backend .MapHub<EnergyHub>("/hub/energy")
      .build();
  }

  public startConnection(): Promise<void> {
    return this.hubConnection.start();
  }

  public onEnergyUpdate(callback: (data: any) => void) {
    this.hubConnection.on('EnergyUpdate', callback);
  }

  // For device/group updates:
  public onDeviceUpdated(callback: (device: any) => void) {
    this.hubConnection.on('DeviceUpdated', callback);
  }

  public onGroupUpdated(callback: (group: any) => void) {
    this.hubConnection.on('GroupUpdated', callback);
  }
}

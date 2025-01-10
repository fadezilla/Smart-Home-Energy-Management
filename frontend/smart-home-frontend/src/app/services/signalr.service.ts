import * as signalR from '@microsoft/signalr';
import { environment } from '../environments/environment';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, Inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;

  constructor(
    @Inject(PLATFORM_ID) private platformId: object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${environment.apiUrl}/hub/energy`)
        .build();
    }
  }

  public startConnection(): Promise<void> {
    if (this.hubConnection) {
      return this.hubConnection.start();
    }
    return Promise.resolve(); // do nothing if server
  }

  public onEnergyUpdate(callback: (data: any) => void) {
    if (this.hubConnection) {
      this.hubConnection.on('EnergyUpdate', callback);
    }
  }

  // For device/group updates:
  public onDeviceUpdated(callback: (device: any) => void) {
    if(this.hubConnection){
      this.hubConnection.on('DeviceUpdated', callback);
    }
  }

  public onGroupUpdated(callback: (group: any) => void) {
    if(this.hubConnection){
      this.hubConnection.on('GroupUpdated', callback);
    }
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

export interface Device {
  deviceId: number;
  name: string;
  isOn: boolean;
  energyConsumptionRate: number;
}

@Injectable({ providedIn: 'root' })
export class DeviceService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllDevices() {
    return this.http.get<Device[]>(`${this.apiUrl}/api/devices`);
  }

  toggleDevice(deviceId: number) {
    return this.http.post<Device>(`${this.apiUrl}/api/devices/${deviceId}/toggle`, {});
  }
}

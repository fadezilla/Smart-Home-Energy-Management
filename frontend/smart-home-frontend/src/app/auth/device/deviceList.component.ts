import { Component, OnInit } from '@angular/core';
import { DeviceService, Device } from '../../services/device.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-device-list',
  standalone: true,
  template: `
    <h2>Devices</h2>
    <div *ngFor="let device of devices">
      <span>{{ device.name }}</span> -
      <span>{{ device.isOn ? 'On' : 'Off' }}</span>
      <button (click)="toggle(device)">Toggle</button>
    </div>
  `,
  imports: [CommonModule] 
})
export class DeviceListComponent implements OnInit {
  devices: Device[] = [];

  constructor(private deviceService: DeviceService) {}

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.deviceService.getAllDevices().subscribe(devs => this.devices = devs);
  }

  toggle(device: Device) {
    this.deviceService.toggleDevice(device.deviceId).subscribe(updated => {
      // Update local array or re-fetch
      this.loadDevices();
    });
  }
}

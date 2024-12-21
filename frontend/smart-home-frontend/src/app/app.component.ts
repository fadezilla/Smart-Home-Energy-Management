import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalRService } from './services/signalr.service'; // Adjust path as needed

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'smart-home-frontend';

  constructor(private signalRService: SignalRService) {}

  ngOnInit(): void {
    this.signalRService.startConnection()
      .then(() => {
        console.log('SignalR connected!');
        this.signalRService.onEnergyUpdate((data) => {
          console.log('Real-time energy data:', data);
        });

        this.signalRService.onDeviceUpdated((device) => {
          console.log('Device updated:', device);
        });

        this.signalRService.onGroupUpdated((group) => {
          console.log('Group updated:', group);
        });
      })
      .catch((err) => console.error('SignalR connection error:', err));
  }
}

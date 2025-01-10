import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../../services/signalr.service';
import { ChartData, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  template: `
    <h2>Real-Time Dashboard</h2>
    <div>{{ currentEnergyUsage }} kWh</div>

    <canvas baseChart
      [data]="lineChartData"
      [options]="lineChartOptions"
      [type]="'line'">
    </canvas>
  `,
  imports: [BaseChartDirective]
})
export class DashboardComponent implements OnInit {
  currentEnergyUsage = 0;

  lineChartData: ChartData<'line'> = {
    labels: [],
    datasets: [
      { data: [], label: 'Energy Usage' },
    ]
  };
  lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
  };

  constructor(private signalR: SignalRService) {}

  ngOnInit() {
    this.signalR.startConnection().then(() => {
      this.signalR.onEnergyUpdate((data) => {
        console.log('Real-time energy data:', data.currentEnergyUsage);
        this.currentEnergyUsage = data.currentEnergyUsage;

        // Append to line chart
        const now = new Date().toLocaleTimeString();
        (this.lineChartData.labels as string[]).push(now);
        this.lineChartData.datasets[0].data.push(data.currentEnergyUsage);
      });
    });
  }
}

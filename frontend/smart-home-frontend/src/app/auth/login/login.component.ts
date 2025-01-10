import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    standalone: true,
    selector: 'app-login',
    template: `
      <h2>Login</h2>
      <form (ngSubmit)="onSubmit()">
        <label>Email:</label>
        <input [(ngModel)]="email" name="email" required>
        <br><br>
        <label>Password:</label>
        <input [(ngModel)]="password" name="password" required type="password">
        <br><br>
        <button type="submit">Login</button>
      </form>
      <p *ngIf="errorMsg" style="color:red">{{ errorMsg }}</p>
    `,
    imports: [NgIf, FormsModule]
  })
export class LoginComponent {
  email = '';
  password = '';
  errorMsg = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        // Store the token
        this.authService.setToken(res.token);
        // Typically you'd decode the token to get the user’s role, etc.
        this.router.navigate(['/dashboard']); // or wherever
      },
      error: (err) => {
        this.errorMsg = 'Invalid email or password';
      }
    });
  }
}

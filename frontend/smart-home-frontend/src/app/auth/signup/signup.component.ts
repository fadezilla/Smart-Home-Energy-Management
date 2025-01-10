import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    standalone: true,
    selector: 'app-signup',
    template: `
      <h2>Sign Up</h2>
      <form (ngSubmit)="onSubmit()">
        <label>Email:</label>
        <input [(ngModel)]="email" name="email" required>
        <br><br>
        <label>Password:</label>
        <input [(ngModel)]="password" name="password" required type="password">
        <br><br>
        <button type="submit">Sign Up</button>
      </form>
      <p *ngIf="errorMsg" style="color:red">{{ errorMsg }}</p>
    `,
    imports: [NgIf, FormsModule]
  })
export class SignupComponent {
  email = '';
  password = '';
  errorMsg = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.authService.signUp(this.email, this.password).subscribe({
      next: () => {
        // Possibly auto-login or redirect to login page
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMsg = 'Email already in use or invalid data';
      }
    });
  }
}

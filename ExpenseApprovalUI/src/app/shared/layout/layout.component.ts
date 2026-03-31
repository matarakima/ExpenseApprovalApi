import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  menuOpen = false;

  constructor(public authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout();
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }
}

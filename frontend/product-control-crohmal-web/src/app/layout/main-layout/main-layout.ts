import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

interface MenuItem {
  label: string;
  route: string;
  roles: string[];
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
  currentUser: any;
  currentRole: string | null = null;

  menuItems: MenuItem[] = [
    {
      label: 'Партії продукції',
      route: '/batches',
      roles: [
        'Адміністратор',
        'Майстер виробництва',
        'Лаборант',
        'Інженер з якості',
        'Працівник складу',
      ],
    },
    {
      label: 'Лабораторні аналізи',
      route: '/analysis',
      roles: ['Адміністратор', 'Лаборант'],
    },
    {
      label: 'Контроль якості',
      route: '/quality',
      roles: ['Адміністратор', 'Інженер з якості'],
    },
    {
      label: 'Сертифікати якості',
      route: '/certificates',
      roles: ['Адміністратор', 'Працівник складу', 'Інженер з якості'],
    },
    {
      label: 'Відвантаження',
      route: '/shipment',
      roles: ['Адміністратор', 'Працівник складу', 'Інженер з якості'],
    },
    {
      label: 'Користувачі',
      route: '/users',
      roles: ['Адміністратор'],
    },
    {
      label: 'Продукти',
      route: '/products',
      roles: ['Адміністратор'],
    },
    {
      label: 'Показники якості',
      route: '/quality-parameters',
      roles: ['Адміністратор'],
    },
    {
      label: 'Специфікації якості',
      route: '/product-quality-specifications',
      roles: ['Адміністратор'],
    },
  ];

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
    this.currentUser = this.authService.getCurrentUser();
    console.log(this.currentUser);
    this.currentRole = this.currentUser.role.name;
    console.log(this.currentRole);
  }

  canShowMenuItem(item: MenuItem): boolean {
    if (!this.currentRole) {
      return false;
    }

    return item.roles.includes(this.currentRole);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

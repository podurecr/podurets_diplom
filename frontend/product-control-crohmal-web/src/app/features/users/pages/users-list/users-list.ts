import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';

import { User } from '../../../../core/models/user.model';
import { UserService } from '../../../../core/services/user.service';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './users-list.html',
  styleUrl: './users-list.css',
})
export class UsersList implements OnInit {
  users: User[] = [];
  searchTerm = '';

  isLoading = false;
  errorMessage = '';

  get filteredUsers(): User[] {
    const term = this.searchTerm.trim().toLowerCase();

    if (!term) {
      return this.users;
    }

    return this.users.filter((user) => user.fullName?.toLowerCase().includes(term));
  }

  clearSearch(): void {
    this.searchTerm = '';
  }

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    console.log('LOAD PRODUCTS START');

    this.isLoading = true;
    this.errorMessage = '';

    this.userService
      .getUsers()
      .pipe(
        finalize(() => {
          console.log('LOAD PRODUCTS FINALIZE');
          this.isLoading = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (data) => {
          console.log('PRODUCTS RESPONSE:', data);
          this.users = data;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('PRODUCTS ERROR:', error);
          this.errorMessage = 'Не вдалося завантажити список користувачів.';
          this.cdr.detectChanges();
        },
      });
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Активний' : 'Неактивний';
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }
}

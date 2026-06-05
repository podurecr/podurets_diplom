import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { UserService } from '../../../../core/services/user.service';
import { User } from '../../../../core/models/user.model';
import { Role } from '../../../../core/models/role.model';

@Component({
  selector: 'app-user-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './user-create.html',
  styleUrl: './user-create.css',
})
export class UserCreate implements OnInit {
  isLoading = false;
  isSaving = false;

  successMessage = '';
  errorMessage = '';

  isEditMode = false;
  userId: number | null = null;

  userForm;

  roles: Role[] = [
    { id: 1, name: 'Адміністратор' },
    { id: 2, name: 'Майстер виробництва' },
    { id: 3, name: 'Лаборант' },
    { id: 4, name: 'Інженер з якості' },
    { id: 5, name: 'Працівник складу' },
  ];

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.userForm = this.fb.group({
      FullName: ['', [Validators.required, Validators.maxLength(200)]],
      Login: ['', [Validators.required, Validators.maxLength(100)]],
      Email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
      PasswordHash: ['', [Validators.required, Validators.minLength(4)]],
      RoleId: [1, [Validators.required]],
      IsActive: [true],
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.isEditMode = true;
      this.userId = id;

      this.userForm.get('PasswordHash')?.clearValidators();
      this.userForm.get('PasswordHash')?.updateValueAndValidity();

      this.loadUser(id);
    }
  }

  loadUser(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService
      .getUserById(id)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (user) => {
          this.userForm.patchValue({
            FullName: user.fullName,
            Login: user.login,
            Email: user.email,
            PasswordHash: '',
            RoleId: user.role?.id ?? this.getRoleIdByName(user.roleName) ?? 1,
            IsActive: user.isActive,
          });
        },
        error: (error) => {
          console.error(error);
          this.errorMessage = 'Не вдалося завантажити дані користувача.';
        },
      });
  }

  saveUser(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    if (this.isEditMode) {
      this.updateUser();
    } else {
      this.createUser();
    }
  }

  createUser(): void {
    const request = this.buildUserRequest();

    this.isSaving = true;

    this.userService
      .createUser(request)
      .pipe(
        finalize(() => {
          this.isSaving = false;
        }),
      )
      .subscribe({
        next: () => {
          this.successMessage = 'Користувача успішно створено.';

          this.userForm.reset({
            FullName: '',
            Login: '',
            Email: '',
            PasswordHash: '',
            RoleId: 1,
            IsActive: true,
          });
        },
        error: (error) => {
          console.error(error);
          this.errorMessage = 'Помилка під час створення користувача.';
        },
      });
  }

  updateUser(): void {
    if (!this.userId) {
      this.errorMessage = 'Некоректний ID користувача.';
      return;
    }

    const request = this.buildUserRequest(this.userId);

    this.isSaving = true;

    this.userService
      .updateUser(this.userId, request)
      .pipe(
        finalize(() => {
          this.isSaving = false;
        }),
      )
      .subscribe({
        next: () => {
          this.successMessage = 'Дані користувача успішно оновлено.';

          setTimeout(() => {
            this.router.navigate(['/users']);
          }, 700);
        },
        error: (error) => {
          console.error(error);
          this.errorMessage = 'Помилка під час оновлення користувача.';
        },
      });
  }

  private buildUserRequest(id = 0): User {
    const formValue = this.userForm.value;

    const selectedRoleId = Number(formValue.RoleId);
    const selectedRole = this.roles.find((role) => role.id === selectedRoleId) ?? this.roles[0];

    return {
      id,
      fullName: formValue.FullName ?? '',
      login: formValue.Login ?? '',
      email: formValue.Email ?? '',
      passwordHash: formValue.PasswordHash ?? '',
      isActive: formValue.IsActive ?? true,
      createdAt: new Date(),

      role: selectedRole,
      roleName: selectedRole.name,

      createdBatches: [],
      analysisResults: [],
      qualityAssessments: [],
      qualityCertificates: [],
      shipmentDecisions: [],
    };
  }

  private getRoleIdByName(roleName?: string | null): number | null {
    if (!roleName) {
      return null;
    }

    return this.roles.find((role) => role.name === roleName)?.id ?? null;
  }
}

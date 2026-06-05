import { Routes } from '@angular/router';
import { Login } from './features/auth/pages/login/login';
import { MainLayout } from './layout/main-layout/main-layout';
import { BatchList } from './features/batches/pages/batch-list/batch-list';
import { BatchCreate } from './features/batches/pages/batch-create/batch-create';
import { BatchDetails } from './features/batches/pages/batch-details/batch-details';
import { QualityAssessment } from './features/quality/pages/quality-assessment/quality-assessment';
import { CertificatePage } from './features/certificates/pages/certificate-page/certificate-page';
import { ShipmentList } from './features/shipment/pages/shipment-list/shipment-list';
import { UsersList } from './features/users/pages/users-list/users-list';
import { UserCreate } from './features/users/pages/user-create/user-create';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { ProductCreate } from './features/products/pages/product-create/product-create';
import { ProductList } from './features/products/pages/product-list/product-list';
import { QualityParameterList } from './features/quality/pages/quality-parameter-list/quality-parameter-list';
import { QualityParameterCreate } from './features/quality/pages/quality-parameter-create/quality-parameter-create';
import { ProductQualitySpecificationList } from './features/quality/pages/product-quality-specification/product-quality-specification-list/product-quality-specification-list';
import { AnalysisList } from './features/analysis/pages/analysis-list/analysis-list';
import { AnalysisWork } from './features/analysis/pages/analysis-work/analysis-work';

export const routes: Routes = [
  {
    path: 'login',
    component: Login,
  },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'users/create',
        component: UserCreate,
      },
      {
        path: 'users/:id/edit',
        component: UserCreate,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'batches',
        component: BatchList,
      },
      {
        path: 'batches/details',
        component: BatchDetails,
      },
      {
        path: 'batches/create',
        component: BatchCreate,
        canActivate: [roleGuard(['Адміністратор', 'Майстер виробництва'])],
      },
      {
        path: 'batches/:id',
        component: BatchDetails,
      },
      /*{
        path: 'batches/:id/edit',
        component: BatchEdit,
        canActivate: [roleGuard(['Адміністратор'])],
      },*/
      {
        path: 'quality',
        component: QualityAssessment,
        canActivate: [roleGuard(['Адміністратор', 'Інженер з якості'])],
      },
      {
        path: 'certificates',
        component: CertificatePage,
        canActivate: [roleGuard(['Адміністратор', 'Працівник складу', 'Інженер з якості'])],
      },
      {
        path: 'shipment',
        component: ShipmentList,
        canActivate: [roleGuard(['Адміністратор', 'Працівник складу', 'Інженер з якості'])],
      },
      {
        path: 'users',
        component: UsersList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: '',
        redirectTo: 'batches',
        pathMatch: 'full',
      },
      {
        path: 'products',
        component: ProductList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'products/create',
        component: ProductCreate,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'products/:id/edit',
        component: ProductList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'quality-parameters',
        component: QualityParameterList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'quality-parameters/create',
        component: QualityParameterCreate,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'quality-parameters/:id/edit',
        component: QualityParameterList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'product-quality-specifications',
        component: ProductQualitySpecificationList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'product-quality-specifications/create',
        component: ProductQualitySpecificationList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'product-quality-specifications/:id/edit',
        component: ProductQualitySpecificationList,
        canActivate: [roleGuard(['Адміністратор'])],
      },
      {
        path: 'analysis',
        component: AnalysisList,
        canActivate: [roleGuard(['Адміністратор', 'Лаборант'])],
      },
      {
        path: 'analysis/work',
        component: AnalysisWork,
        canActivate: [roleGuard(['Адміністратор', 'Лаборант'])],
      },
      {
        path: 'analysis/work/:id',
        component: AnalysisWork,
        canActivate: [roleGuard(['Адміністратор', 'Лаборант'])],
      },
      {
        path: 'quality/batches/:id',
        component: QualityAssessment,
        canActivate: [roleGuard(['Адміністратор', 'Інженер з якості'])],
      },
      {
        path: 'certificates/batches/:id',
        component: CertificatePage,
        canActivate: [roleGuard(['Адміністратор', 'Інженер з якості'])],
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
